using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace IDosGames
{
	public class WalletWindow : MonoBehaviour
	{
		[SerializeField] private WalletView _walletView;

		public async void TransferToken(TransactionDirection direction, VirtualCurrencyID virtualCurrencyID, int amount)
		{
			string transferResult = null;
			if (direction == TransactionDirection.Game)
			{
				transferResult = await WalletService.TransferTokenToGame(virtualCurrencyID, amount);
			}
			else if (direction == TransactionDirection.UsersCryptoWallet)
			{
				transferResult = await WalletService.TransferTokenToUsersCryptoWallet(virtualCurrencyID, amount);
			}

			Debug.Log("TransferResult: " + transferResult);

			ProcessResultMessage(transferResult);
		}

		private static void ProcessResultMessage(string result)
		{
			if (result == null)
			{
				Message.Show("Something went wrong.Ñontact technical support.");
			}

			var resultJson = JsonConvert.DeserializeObject<JObject>(result);

			if (resultJson.ContainsKey(PlayFabReturnMessage.KEY_MESSAGE))
			{
				Message.Show(resultJson[PlayFabReturnMessage.KEY_MESSAGE].ToString());
			}
			else
			{
				Message.Show("Something went wrong.Ñontact technical support.");
			}
		}
	}
}