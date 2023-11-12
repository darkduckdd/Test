using PlayFab;
using PlayFab.ClientModels;
using PlayFab.EconomyModels;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

namespace IDosGames
{
	public class PlayFabIAPValidator
	{
		private static PlayFabIAPValidator _instance;

		public static event Action<string> PurchaseValidated;
		public static event Action VIPSubscriptionValidated;

		public PlayFabIAPValidator()
		{
			_instance = this;

			IAPService.PurchaseCompleted += ValidatePurchase;
		}

		~PlayFabIAPValidator()
		{
			IAPService.PurchaseCompleted -= ValidatePurchase;
		}

		[RuntimeInitializeOnLoadMethod]
		private static void Initialize()
		{
			_instance = new();
		}

		private void ValidatePurchase(PurchaseEventArgs purchaseEvent)
		{
			var receipt = IAPPurchase.ConvertFromJson(purchaseEvent.purchasedProduct.receipt);

			string productID = string.Empty;

#if UNITY_ANDROID

			productID = receipt.PayloadData.JsonPurchaseData.productId;

			PlayFabClientAPI.ValidateGooglePlayPurchase(new ValidateGooglePlayPurchaseRequest()
			{
				CurrencyCode = purchaseEvent.purchasedProduct.metadata.isoCurrencyCode,
				PurchasePrice = (uint)(purchaseEvent.purchasedProduct.metadata.localizedPrice * 100),
				ReceiptJson = receipt.PayloadData.json,
				Signature = receipt.PayloadData.signature
			},
			result =>
			{
				PurchaseValidated?.Invoke(productID);
			},
			error => Debug.LogWarning("Validation failed: " + error.GenerateErrorReport()));

#elif UNITY_IOS

			productID = purchaseEvent.purchasedProduct.definition.id;

			PlayFabClientAPI.ValidateIOSReceipt(new ValidateIOSReceiptRequest()
			{
				CurrencyCode = purchaseEvent.purchasedProduct.metadata.isoCurrencyCode,
				PurchasePrice = (int)(purchaseEvent.purchasedProduct.metadata.localizedPrice * 100),
				ReceiptData = receipt.Payload
			},
			result =>
			{
				PurchaseValidated?.Invoke(productID);
			},
			error => Debug.LogWarning("Validation failed: " + error.GenerateErrorReport()));
#endif
		}

		public static void ValidateVIPSubscriptionPurchase(string json)
		{
			///if (string.IsNullOrEmpty(json)) { return; }

#if UNITY_ANDROID

			/*
			JsonIAPPayloadData payloadData = JsonIAPPayloadData.ConvertFromJson(json);

			PlayFabClientAPI.ValidateGooglePlayPurchase(new ValidateGooglePlayPurchaseRequest()
			{
				ReceiptJson = payloadData.json,
				Signature = payloadData.signature
			},
			result =>
			{
				VIPSubscriptionValidated?.Invoke();
			},
			error => Debug.LogWarning("Validation failed: " + error.GenerateErrorReport()));
			*/

			PlayFabEconomyAPI.RedeemGooglePlayInventoryItems(new RedeemGooglePlayInventoryItemsRequest
			{
				Purchases = new List<GooglePlayProductPurchase> { new GooglePlayProductPurchase { ProductId = "vip_subscription" } }
			},
			result =>
			{
				Debug.Log("Purchase successfully redeemed.");

				VIPSubscriptionValidated?.Invoke();
			},
			error =>
			{
				Debug.LogError($"Failed to redeem purchase: {error.ErrorMessage}");
			});

#elif UNITY_IOS

			var appleReceipt = IAPPurchase.ConvertFromJson(json);

			PlayFabClientAPI.ValidateIOSReceipt(new ValidateIOSReceiptRequest()
			{
				ReceiptData = appleReceipt.Payload
			},
			result =>
			{
				VIPSubscriptionValidated?.Invoke();
			},
			error => Debug.LogWarning("Validation failed: " + error.GenerateErrorReport()));

#endif
		}
	}
}