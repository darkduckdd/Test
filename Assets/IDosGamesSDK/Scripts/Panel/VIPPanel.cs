using Newtonsoft.Json.Linq;
using UnityEngine;

namespace IDosGames
{
	public class VIPPanel : ShopPanel
	{
		private const string PANEL_CLASS = PlayFabItemClass.VIP;

		[SerializeField] private VIPItem _itemForRM;
		[SerializeField] private VIPItem _itemForVirtualCurrency;

		public override void InitializePanel()
		{
			var RMProducts = ShopSystem.ProductsForRealMoney;
			var VCProducts = ShopSystem.ProductsForVirtualCurrency;

			if (RMProducts != null)
			{
				InitializeRMProduct(RMProducts);
			}

			if (VCProducts != null)
			{
				InitializeVCProduct(VCProducts);
			}
		}

		private void InitializeRMProduct(JArray RMProducts)
		{
			foreach (var product in RMProducts)
			{
				var itemClass = $"{product[JsonProperty.ITEM_CLASS]}";

				if (itemClass != PANEL_CLASS)
				{
					continue;
				}

				var itemID = $"{product[JsonProperty.ITEM_ID]}";

				var price = GetPriceInRealMoney($"{product[JsonProperty.PRICE_RM]}");

				_itemForRM.Fill(() => ShopSystem.BuyForRealMoney(itemID), $"${price}");
			}
		}

		private void InitializeVCProduct(JArray VCProducts)
		{
			foreach (var product in VCProducts)
			{
				var itemClass = $"{product[JsonProperty.ITEM_CLASS]}";

				if (itemClass != PANEL_CLASS)
				{
					continue;
				}

				var itemID = $"{product[JsonProperty.ITEM_ID]}";

				var price = GetPriceInRealMoney($"{product[JsonProperty.PRICE_RM]}");

				var currencyIcon = Resources.Load<Sprite>(product[JsonProperty.CURRENCY_IMAGE_PATH].ToString());

				var currencyID = GetVirtualCurrencyID($"{product[JsonProperty.CURRENCY_ID]}");

				price = GetPriceInVirtualCurrency(price, currencyID);

				_itemForVirtualCurrency.Fill(() => ShopSystem.BuyForVirtualCurrency(itemID, currencyID, price), $"{price:N0}");
			}
		}
	}
}