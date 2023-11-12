using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

namespace IDosGames
{
	public class DailyOfferShopPanel : ShopPanel
	{
		[SerializeField] private ShopFreeItem _dailyFreeOfferPrefab;
		[SerializeField] private ShopItem _dailyPaidOfferPrefab;
		[SerializeField] private Transform _content;
		[SerializeField] private DailyOffersTimer _timer;
		[SerializeField] private SpinWindow _spinWindow;
		[SerializeField] private FreeSpinButton _freeSpinButton;

		private DateTime _endDate;

		public override void InitializePanel()
		{
			var freeProducts = ShopSystem.DailyFreeProducts;
			var offerData = ShopSystem.DailyOfferData;

			var products = offerData[JsonProperty.PRODUCTS];

			_endDate = GetEndDateTime(offerData);
			UpdateTimer(_endDate);

			if (products == null)
			{
				return;
			}

			foreach (Transform child in _content)
			{
				Destroy(child.gameObject);
			}

			InitializeFreeProducts(freeProducts);
			InitializePaidProducts(products);
		}

		private void InitializeFreeProducts(JArray products)
		{
			var playerData = PlayFabDataService.GetPlayerData(PlayerDataKey.shop_daily_free_products);

			if (IsNeedUpdateDailyFreeProducts(playerData))
			{
				UpdateDailyFreeProducts();
				return;
			}

			foreach (var product in products)
			{
				if ($"{product[JsonProperty.ENABLED]}" != JsonProperty.ENABLED_VALUE)
				{
					continue;
				}

				var productItem = Instantiate(_dailyFreeOfferPrefab, _content);

				var itemID = $"{product[JsonProperty.ITEM_ID]}";

				var icon = Resources.Load<Sprite>(product[JsonProperty.IMAGE_PATH].ToString());

				var title = $"{product[JsonProperty.NAME]}";

				var itemClass = $"{product[JsonProperty.ITEM_CLASS]}";

				int productAmountInPlayer = GetProductAmountInPlayer(itemID, playerData);
				int productAmountInOffer = GetProductAmountInOffer(itemID, products);

				string quantityAmount = $"{productAmountInPlayer}/{productAmountInOffer}";

				productItem.View.SetQuantity(quantityAmount);

				bool isNeedShowAd = IsNeedToShowAd(productAmountInOffer, productAmountInPlayer);
				bool isNeedBlock = productAmountInPlayer < 1;

				if (isNeedBlock)
				{
					productItem.View.Block();
				}
				else
				{
					productItem.View.UnBlock();
					productItem.View.SetActiveAddIcon(isNeedShowAd);
				}

				Action onclickCalback;

				if (itemClass == PlayFabItemClass.SPIN_TICKET)
				{
					onclickCalback = () => _spinWindow.OpenFreeSpin();
					productItem.View.DisableTextAmountToGrant();

					Action tryToSpinAction = () => _spinWindow.TryToFreeSpin(isNeedShowAd);
					_freeSpinButton.Set(tryToSpinAction, _endDate, quantityAmount, isNeedShowAd, isNeedBlock);
				}
				else
				{
					onclickCalback = () => ShopSystem.TryGetDailyFreeReward(itemID, isNeedShowAd);
					productItem.View.SetAmountToGrant(GetItemAmountToGrant(product));
				}

				productItem.Fill(onclickCalback, title, string.Empty, icon);
			}
		}

		private bool IsNeedToShowAd(int productAmountInOffer, int productAmountInPlayer)
		{
			if (UserInventory.HasVIPStatus)
			{
				return false;
			}

			bool isNeed = false;

			if (productAmountInOffer <= 1 || productAmountInOffer - productAmountInPlayer > 0)
			{
				isNeed = true;
			}

			return isNeed;
		}

		private string GetItemAmountToGrant(JToken product)
		{
			string amount = string.Empty;

			JArray items = (JArray)product[JsonProperty.ITEMS_TO_GRANT];

			if (items.Count > 0)
			{
				amount = $"{items[0][JsonProperty.AMOUNT]}";
			}

			return amount;
		}

		private int GetProductAmountInPlayer(string itemID, string playerData)
		{
			int amount = 0;

			var jsonData = JsonConvert.DeserializeObject<JObject>(playerData);

			if ($"{jsonData}" != string.Empty)
			{
				var playerProducts = jsonData[JsonProperty.PRODUCTS];

				foreach (var product in playerProducts)
				{
					if ($"{product[JsonProperty.ITEM_ID]}" == itemID)
					{
						int.TryParse($"{product[JsonProperty.AMOUNT]}", out amount);
						break;
					}
				}
			}

			return amount;
		}

		private int GetProductAmountInOffer(string itemID, JArray products)
		{
			int amount = 0;

			foreach (var product in products)
			{
				if ($"{product[JsonProperty.ITEM_ID]}" == itemID)
				{
					int.TryParse($"{product[JsonProperty.AMOUNT]}", out amount);
					break;
				}
			}

			return amount;
		}

		private void UpdateDailyFreeProducts()
		{
			PlayFabService.ExecuteCloudFunction(PlayFabCloudFunctionName.UPDATE_DAILY_FREE_PRODUCTS, (result) => PlayFabDataService.RequestAllData());
		}

		private bool IsNeedUpdateDailyFreeProducts(string playerData)
		{
			if (playerData == string.Empty)
			{
				return true;
			}

			var jsonData = JsonConvert.DeserializeObject<JObject>(playerData);

			var playerLastUpdateDate = GetEndDateTime(jsonData);

			if (_endDate > playerLastUpdateDate)
			{

				return true;
			}

			return false;
		}

		private void InitializePaidProducts(JToken products)
		{
			foreach (var product in products)
			{
				var productItem = Instantiate(_dailyPaidOfferPrefab, _content);

				var itemID = $"{product[JsonProperty.ITEM_ID]}";

				var price = GetPriceInRealMoney($"{product[JsonProperty.PRICE_RM]}");

				var icon = Resources.Load<Sprite>(product[JsonProperty.IMAGE_PATH].ToString());

				var title = $"{product[JsonProperty.NAME]}";

				var currencyIcon = Resources.Load<Sprite>(product[JsonProperty.CURRENCY_IMAGE_PATH].ToString());

				var currencyID = GetVirtualCurrencyID($"{product[JsonProperty.CURRENCY_ID]}");

				price = GetPriceInVirtualCurrency(price, currencyID);

				Action onclickCalback = () => ShopSystem.PopUpSystem.ShowConfirmationPopUp(() => ShopSystem.BuyDailyItem(itemID, currencyID, price), title, $"{price}", currencyIcon);

				productItem.Fill(onclickCalback, title, $"{price:N0}", icon, currencyIcon);
			}
		}

		private DateTime GetEndDateTime(JToken jToken)
		{
			DateTime.TryParse($"{jToken[JsonProperty.END_DATE]}", out DateTime endDate);

			return endDate;
		}

		private void UpdateTimer(DateTime endDate)
		{
			_timer.Set(endDate);
		}
	}
}