using System;
using UnityEngine;

namespace IDosGames
{
	public abstract class ShopPanel : MonoBehaviour
	{
		private void OnEnable()
		{
			PlayFabDataService.AllDataUpdated += InitializePanel;
		}

		private void OnDisable()
		{
			PlayFabDataService.AllDataUpdated -= InitializePanel;
		}

		private void Start()
		{
			InitializePanel();
		}

		public abstract void InitializePanel();

		public float GetPriceInRealMoney(string data)
		{
			float.TryParse(data, out float price);

			price /= 100;

			return price;
		}

		public float GetPriceInVirtualCurrency(float price, VirtualCurrencyID currencyID)
		{
			switch (currencyID)
			{
				case VirtualCurrencyID.CO:
					price = CryptocurrencyPrices.ConverRMtoIGC(price);
					break;
				default:
				case VirtualCurrencyID.IG:
					price = CryptocurrencyPrices.ConverRMtoIGT(price);
					break;
			}

			return price;
		}

		public VirtualCurrencyID GetVirtualCurrencyID(string currencyID)
		{
			Enum.TryParse(currencyID, true, out VirtualCurrencyID virtualCurrencyID);

			return virtualCurrencyID;
		}
	}
}