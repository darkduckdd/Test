using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;

namespace IDosGames
{
	public class ShopSystem : MonoBehaviour
	{
		private static ShopSystem _instance;

		private const string PLAYER_PREFS_AFTER_IAP_NOT_GRANTED_PRODUCT = "NOT_GRANTED_IAP_PRODUCT";
		private const int MAX_ATTEMPTS_COUNT_FOR_GRANT_IAP_ITEMS = 3;
		private static int _attemptsCountForGrantIAPItemsAfterError = 0;

		[SerializeField] private ShopPopUpSystem _popUpSystem;
		public static ShopPopUpSystem PopUpSystem => _instance._popUpSystem;

		public static JArray ProductsForRealMoney { get; private set; }
		public static JArray ProductsForVirtualCurrency { get; private set; }
		public static JArray SpecialOfferProducts { get; private set; }
		public static JObject DailyOfferData { get; private set; }
		public static JArray DailyFreeProducts { get; private set; }

		private static string _iapProcessedProductID;
		private static string _freeProductID;

		private void Awake()
		{
			if (_instance == null || ReferenceEquals(this, _instance))
			{
				_instance = this;
				DontDestroyOnLoad(gameObject);
			}

			CheckForItemsGrantedAfterIAPPurchase();
		}

		private void OnEnable()
		{
			CryptocurrencyPrices.PricesUpdated += UpdateProductsData;
			PlayFabIAPValidator.PurchaseValidated += GrantItemsAfterIAPPurchase;
		}

		private void OnDisable()
		{
			CryptocurrencyPrices.PricesUpdated -= UpdateProductsData;
			PlayFabIAPValidator.PurchaseValidated -= GrantItemsAfterIAPPurchase;
		}

		public static void BuyForRealMoney(string ID)
		{
			IAPService.PurchaseProductByID(ID);
			_iapProcessedProductID = ID;
		}

		public static void BuyForVirtualCurrency(string ID, VirtualCurrencyID currencyID, float price)
		{
			if (UserInventory.GetVirtualCurrencyAmount(currencyID) < price)
			{
				Message.Show(PlayFabReturnMessage.SHOP_MESSAGE_CODE_NOT_ENOUGH_FUNDS);
				PopUpSystem.ShowCurrencyPopUp();
				return;
			}

			var parameter = new Dictionary<string, object>()
			{
				{
					PlayFabCloudFunctionParameter.ITEM_ID, ID
				}
			};

			PlayFabService.ExecuteCloudFunction(PlayFabCloudFunctionName.BUY_ITEM_FOR_VIRTUAL_CURRENCY, OnSuccessPurchase, OnErrorPurchase, functionParameter: parameter);
		}

		public static void BuySpecialItem(string ID, VirtualCurrencyID currencyID, float price)
		{
			if (UserInventory.GetVirtualCurrencyAmount(currencyID) < price)
			{
				Message.Show(PlayFabReturnMessage.SHOP_MESSAGE_CODE_NOT_ENOUGH_FUNDS);
				PopUpSystem.ShowCurrencyPopUp();
				return;
			}

			var parameter = new Dictionary<string, object>()
			{
				{
					PlayFabCloudFunctionParameter.ITEM_ID, ID
				}
			};

			PlayFabService.ExecuteCloudFunction(PlayFabCloudFunctionName.BUY_ITEM_SPECIAL_OFFER, OnSuccessSpecialItemPurchase, OnErrorPurchase, functionParameter: parameter);
		}

		public static void BuyDailyItem(string ID, VirtualCurrencyID currencyID, float price)
		{
			if (UserInventory.GetVirtualCurrencyAmount(currencyID) < price)
			{
				Message.Show(PlayFabReturnMessage.SHOP_MESSAGE_CODE_NOT_ENOUGH_FUNDS);
				PopUpSystem.ShowCurrencyPopUp();
				return;
			}

			var parameter = new Dictionary<string, object>()
			{
				{
					PlayFabCloudFunctionParameter.ITEM_ID, ID
				}
			};

			PlayFabService.ExecuteCloudFunction(PlayFabCloudFunctionName.BUY_ITEM_DAILY_OFFER, OnSuccessPurchase, OnErrorPurchase, functionParameter: parameter);
		}

		public static void TryGetDailyFreeReward(string ID, bool showAd)
		{
			_freeProductID = ID;

			if (showAd && !UserInventory.HasVIPStatus)
			{
				if (AdMediation.Instance.ShowRewardedVideo(GetDailyFreeReward))
				{
					Debug.Log("Show rewarded video.");
				}
				else
				{
					Message.Show("Ad is not ready.");
				}
			}
			else
			{
				GetDailyFreeReward();
			}
		}

		private static void CheckForItemsGrantedAfterIAPPurchase()
		{
			if (PlayerPrefs.HasKey(PLAYER_PREFS_AFTER_IAP_NOT_GRANTED_PRODUCT))
			{
				_iapProcessedProductID = PlayerPrefs.GetString(PLAYER_PREFS_AFTER_IAP_NOT_GRANTED_PRODUCT);
				GrantItemsAfterIAPPurchase(_iapProcessedProductID);
			}
		}

		private void UpdateProductsData()
		{
			var dataProductsForRealMoney = PlayFabDataService.GetTitleData(TitleDataKey.products_for_real_money);
			var dataProductsForVirtualCurrency = PlayFabDataService.GetTitleData(TitleDataKey.products_for_virtual_currency);
			var dataSpecialProducts = PlayFabDataService.GetTitleData(TitleDataKey.shop_special_products);
			var dataDailyOffer = PlayFabDataService.GetTitleData(TitleDataKey.shop_daily_products);
			var dataDailyFreeProducts = PlayFabDataService.GetTitleData(TitleDataKey.shop_daily_free_products);

			ProductsForRealMoney = JsonConvert.DeserializeObject<JArray>(dataProductsForRealMoney);
			ProductsForRealMoney ??= new JArray();

			ProductsForVirtualCurrency = JsonConvert.DeserializeObject<JArray>(dataProductsForVirtualCurrency);
			ProductsForVirtualCurrency ??= new JArray();

			SpecialOfferProducts = JsonConvert.DeserializeObject<JArray>(dataSpecialProducts);
			SpecialOfferProducts ??= new JArray();

			DailyOfferData = JsonConvert.DeserializeObject<JObject>(dataDailyOffer);
			DailyOfferData ??= new JObject();

			DailyFreeProducts = JsonConvert.DeserializeObject<JArray>(dataDailyFreeProducts);
			DailyFreeProducts ??= new JArray();
		}

		private static void GrantItemsAfterIAPPurchase(string ID)
		{
			var parameter = new Dictionary<string, object>()
			{
				{
					PlayFabCloudFunctionParameter.ITEM_ID, ID
				}
			};

			PlayFabService.ExecuteCloudFunction(PlayFabCloudFunctionName.GRANT_ITEM_AFTER_IAP_PURCHASE, OnSuccessGrantItemsAfterIAP, OnErrorGrantItemsAfterIAP, functionParameter: parameter);
		}

		private static void OnSuccessGrantItemsAfterIAP(ExecuteCloudScriptResult result)
		{
			OnSuccessPurchase(result);

			if (PlayerPrefs.HasKey(PLAYER_PREFS_AFTER_IAP_NOT_GRANTED_PRODUCT))
			{
				PlayerPrefs.DeleteKey(PLAYER_PREFS_AFTER_IAP_NOT_GRANTED_PRODUCT);
				PlayerPrefs.Save();
			}
		}

		private static void OnErrorGrantItemsAfterIAP(PlayFabError error)
		{
			PlayerPrefs.SetString(PLAYER_PREFS_AFTER_IAP_NOT_GRANTED_PRODUCT, _iapProcessedProductID);

			if (_attemptsCountForGrantIAPItemsAfterError < MAX_ATTEMPTS_COUNT_FOR_GRANT_IAP_ITEMS)
			{
				CheckForItemsGrantedAfterIAPPurchase();
				_attemptsCountForGrantIAPItemsAfterError++;
			}
			else
			{
				OnErrorPurchase(error);
			}
		}

		private static void GetDailyFreeReward(bool adCompleted = true)
		{
			var parameter = new Dictionary<string, object>()
			{
				{
					PlayFabCloudFunctionParameter.ITEM_ID, _freeProductID
				}
			};

			PlayFabService.ExecuteCloudFunction(PlayFabCloudFunctionName.GET_DAILY_FREE_REWARD, OnSuccessSpecialItemPurchase, OnErrorPurchase, functionParameter: parameter);
		}

		private static void OnSuccessPurchase(ExecuteCloudScriptResult result)
		{
			if (result == null || result.FunctionResult == null)
			{
				OnErrorPurchase(null);
			}
			else
			{
				JObject resultData = JsonConvert.DeserializeObject<JObject>(result.FunctionResult.ToString());
				if (resultData[PlayFabReturnMessage.KEY_MESSAGE] != null)
				{
					Message.Show(resultData[PlayFabReturnMessage.KEY_MESSAGE].ToString());
					PlayFabService.RequestUserInventory();
				}
			}
		}

		private static void OnSuccessSpecialItemPurchase(ExecuteCloudScriptResult result)
		{
			OnSuccessPurchase(result);
			PlayFabDataService.RequestAllData();
		}

		private static void OnErrorPurchase(PlayFabError error)
		{
			Message.Show("Something went wrong. " + error);
		}
	}
}