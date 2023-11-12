using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using UnityEngine;

namespace IDosGames
{
	public class CryptocurrencyPrices
	{
		private static CryptocurrencyPrices _instance;

		public static float IGT { get; private set; }
		public static float IGC { get; private set; }

		private static int _exchangeDivider = 1;

		public static float ExchangeRate { get; private set; }

		public static event Action PricesUpdated;

		public CryptocurrencyPrices()
		{
			_instance = this;

			PlayFabDataService.AllDataUpdated += InitializePrices;
		}

		~CryptocurrencyPrices()
		{
			PlayFabDataService.AllDataUpdated -= InitializePrices;
		}

		[RuntimeInitializeOnLoadMethod]
		private static void Initialize()
		{
			_instance = new();
		}

		private void InitializePrices()
		{
			var pricesData = PlayFabDataService.GetTitleData(TitleDataKey.cryptocurrency_prices);

			if (pricesData == null)
			{
				return;
			}

			var prices = JsonConvert.DeserializeObject<JObject>(pricesData);

			float.TryParse(prices[JsonProperty.IGC].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out float igc);
			IGC = igc;
			float.TryParse(prices[JsonProperty.IGT].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out float igt);
			IGT = igt;
			int.TryParse(prices[JsonProperty.EXCHANCE_DIVIDER].ToString(), out _exchangeDivider);

			ExchangeRate = GetExchangeRate();

			PricesUpdated?.Invoke();
		}

		private float GetExchangeRate()
		{
			if (IGT > 0 && IGC > 0)
			{
				return (float)Math.Round(IGT / IGC, 2);
			}

			return float.MaxValue;
		}

		public static int ConverRMtoIGT(float rm)
		{
			if (rm > 0 && IGT > 0)
			{
				return (int)Math.Round(rm / IGT);
			}

			return int.MaxValue;
		}

		public static int ConverRMtoIGTwithDivider(float rm)
		{
			if (rm > 0 && IGT > 0)
			{
				return (int)Math.Round((rm / IGT) / _exchangeDivider);
			}

			return int.MaxValue;
		}

		public static int ConverRMtoIGC(float rm)
		{
			if (rm > 0 && IGC > 0)
			{
				return (int)Math.Round(rm / IGC);
			}

			return int.MaxValue;
		}

		public static int ConverRMtoIGCwithDivider(float rm)
		{
			if (rm > 0 && IGC > 0)
			{
				return (int)System.Math.Round((rm / IGC) / _exchangeDivider);
			}

			return int.MaxValue;
		}

		public static int ConverIGCtoIGT(int igc)
		{
			if (igc > 0 && IGT > 0 && IGC > 0)
			{
				return (int)System.Math.Round((igc * IGC) / IGT, 0);
			}

			return int.MaxValue;
		}

		public static int ConverIGTtoIGC(int igt)
		{
			if (igt > 0 && IGT > 0 && IGC > 0)
			{
				return (int)System.Math.Round((igt * IGT) / IGC, 0);
			}

			return int.MaxValue;
		}
	}
}