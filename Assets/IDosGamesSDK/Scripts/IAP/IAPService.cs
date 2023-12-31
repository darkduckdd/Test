using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace IDosGames
{
	public class IAPService : IDetailedStoreListener
	{
		public const string DEFAULT_ENVIRONMENT = "production";

		private static IAPService _instance;

		public static bool IsInitialized => _instance._storeController != null;

		public static event Action<PurchaseEventArgs> PurchaseCompleted;
		public static event Action<PurchaseFailureReason> PurchaseFailed;

		private IStoreController _storeController;
		private IExtensionProvider _extensionProvider;

		public IAPService()
		{
			_instance = this;

			PlayFabDataService.AllDataUpdated += InitializeStoreProducts;
		}

		~IAPService()
		{
			PlayFabDataService.AllDataUpdated -= InitializeStoreProducts;
		}

		[RuntimeInitializeOnLoadMethod]
		private static void Initialize()
		{
			_instance = new();
		}

		[RuntimeInitializeOnLoadMethod]
		private static async void InitializeUnityServices()
		{
			try
			{
				var options = new InitializationOptions().SetEnvironmentName(DEFAULT_ENVIRONMENT);
				await UnityServices.InitializeAsync(options);
			}
			catch (Exception)
			{
				Debug.LogError("An error occurred during Unity services initialization. (IAP)");
			}
		}

		private void InitializeStoreProducts()
		{
			if (IsInitialized)
			{
				return;
			}

			var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

			AddProductsToBuilder(builder);

			UnityPurchasing.Initialize(this, builder);
		}

		private void AddProductsToBuilder(ConfigurationBuilder builder)
		{
			var productsData = PlayFabDataService.GetTitleData(TitleDataKey.products_for_real_money);

			var products = JsonConvert.DeserializeObject<JArray>(productsData);

			products ??= new JArray();

			foreach (var product in products)
			{
				if (Enum.TryParse($"{product[JsonProperty.PRODUCT_TYPE]}", true, out ProductType productType))
				{
					builder.AddProduct($"{product[JsonProperty.ITEM_ID]}", productType);
				}
			}
		}

		public static void PurchaseProductByID(string productID)
		{
			if (!IsInitialized)
			{
				return;
			}

			_instance._storeController.InitiatePurchase(productID);
		}

		public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
		{
			_storeController = controller;
			_extensionProvider = extensions;
		}

		public void OnInitializeFailed(InitializationFailureReason error)
		{
			Debug.LogWarning($"UnityPurchasing initialize failed. Error: {error}");
		}

		public void OnInitializeFailed(InitializationFailureReason error, string message)
		{
			Debug.LogWarning($"UnityPurchasing initialize failed. Error: {error} | Message: {message}");
		}

		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
		{
			if (!IsInitialized)
			{
				return PurchaseProcessingResult.Complete;
			}

			if (purchaseEvent.purchasedProduct == null)
			{
				Debug.LogWarning("Attempted to process purchase with unknown product.");
				return PurchaseProcessingResult.Complete;
			}

			if (string.IsNullOrEmpty(purchaseEvent.purchasedProduct.receipt))
			{
				Debug.LogWarning("Attempted to process purchase with no receipt.");
				return PurchaseProcessingResult.Complete;
			}

			Debug.Log("Processing transaction: " + purchaseEvent.purchasedProduct.transactionID);

			PurchaseCompleted?.Invoke(purchaseEvent);

			return PurchaseProcessingResult.Complete;
		}

		public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
		{
			Debug.LogWarning($"Purchase Failed: Product: {product.definition.storeSpecificId}, Failure Reason: {failureReason}");

			if (failureReason != PurchaseFailureReason.UserCancelled)
			{
				PurchaseFailed?.Invoke(failureReason);
			}
		}

		public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
		{
			Debug.LogWarning($"Purchase Failed: Product: {product.definition.storeSpecificId}, Failure Description: {failureDescription}");
		}

		public void RestorePurchasesIOS()
		{
			_extensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions((bool resultBool, string resultString) => { });
		}
	}
}