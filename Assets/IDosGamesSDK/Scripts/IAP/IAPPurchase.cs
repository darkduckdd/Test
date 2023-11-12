using Newtonsoft.Json;

namespace IDosGames
{
	public class IAPPurchase
	{
		public string Store;

		public string TransactionID;
		public string Payload;

		public JsonIAPPayloadData PayloadData;

		public static IAPPurchase ConvertFromJson(string json)
		{
			var purchase = JsonConvert.DeserializeObject<IAPPurchase>(json);

			purchase.PayloadData = JsonIAPPayloadData.ConvertFromJson(purchase.Payload);

			return purchase;
		}
	}
}