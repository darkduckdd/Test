using Firebase.Analytics;
using System;
using UnityEngine;
using UnityEngine.Purchasing;

namespace IDosGames
{
	public class Analytics : MonoBehaviour
	{
		private static Analytics _instance;

		private void Awake()
		{
			if (_instance == null || ReferenceEquals(this, _instance))
			{
				_instance = this;
				DontDestroyOnLoad(gameObject);
			}
		}

		private void OnEnable()
		{
			IAPService.PurchaseCompleted += ReportIAPRevenue;
			AdMediation.RevenueDataReceived += ReportAdRevenue;
		}

		private void OnDisable()
		{
			IAPService.PurchaseCompleted -= ReportIAPRevenue;
			AdMediation.RevenueDataReceived -= ReportAdRevenue;
		}

		public static void Send(string name, string parameterName = "", string parameterValue = "")
		{
			FirebaseAnalytics.LogEvent(name, parameterName, parameterValue);
			AppMetricaAnalytics.Instance.ReportEvent(name);
		}

		private void ReportIAPRevenue(PurchaseEventArgs args)
		{
			if (args.purchasedProduct.receipt == null)
			{
				return;
			}

			var receipt = IAPPurchase.ConvertFromJson(args.purchasedProduct.receipt);
			string currency = args.purchasedProduct.metadata.isoCurrencyCode;
			decimal price = args.purchasedProduct.metadata.localizedPrice;

			YandexAppMetricaRevenue revenue = new(price, currency);
			YandexAppMetricaReceipt yaReceipt = new();

#if UNITY_ANDROID
			JsonIAPPayloadData payloadData = JsonIAPPayloadData.ConvertFromJson(receipt.Payload);
			yaReceipt.Signature = payloadData.signature;
			yaReceipt.Data = payloadData.json;
#elif UNITY_IOS
			yaReceipt.TransactionID = receipt.TransactionID;
            yaReceipt.Data = receipt.Payload;
#endif
			revenue.Receipt = yaReceipt;

			AppMetricaAnalytics.Instance.ReportRevenue(revenue);
		}

		private void ReportAdRevenue(AdRevenueData data)
		{
			ReportAdRevenueFirebase(data);
			ReportAdRevenueAppMetrica(data);
		}

		private void ReportAdRevenueFirebase(AdRevenueData data)
		{
			Parameter[] AdParameters = {
				new Parameter("ad_platform", data.AdPlatform),
				new Parameter("ad_source", data.AdNetwork),
				new Parameter("ad_unit_name", data.AdUnitName),
				new Parameter("ad_format", data.AdType),
				new Parameter("currency", data.Currency),
				new Parameter("value", $"{data.Revenue}")
			};

			FirebaseAnalytics.LogEvent("ad_revenue", AdParameters);
		}

		private void ReportAdRevenueAppMetrica(AdRevenueData data)
		{
			YandexAppMetricaAdRevenue revenue = new((double)data.Revenue, data.Currency)
			{
				AdPlacementName = data.AdPlatform,
				AdNetwork = data.AdNetwork,
				AdUnitName = data.AdUnitName
			};

			Enum.TryParse(data.AdType, out YandexAppMetricaAdRevenue.AdTypeEnum adType);
			revenue.AdType = adType;

			AppMetricaAnalytics.Instance.ReportAdRevenue(revenue);
		}
	}
}