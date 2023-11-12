using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using UnityEngine;
using WalletConnectSharp.Unity;

namespace IDosGames
{
	public static class WalletService
	{
		public static async Task<string> TransferTokenToGame(VirtualCurrencyID virtualCurrencyID, int amount)
		{
			if (!IsWalletConnectReady())
			{
				Debug.LogError("WalletConnect is not ready.");
				return null;
			}

			string walletAddress = WalletConnect.GetActiveAccountAddress();

			if (walletAddress == null)
			{
				Debug.LogError("WalletConnect active account is not ready.");
				return null;
			}

			var usersWallet = WalletConnect.GetActiveAccountAddress();
			var companyWalletAddress = BlockchainSettings.CompanyCryptoWallet;

			var transactionHash = await WalletConnectBlockchainService.TransferTokenAndGetHash(usersWallet, companyWalletAddress, virtualCurrencyID, amount);

			var requestBody = new JObject
			{
				{ "AuthContext", JsonConvert.SerializeObject(PlayFabAuthService.AuthContext) },
				{ "TransactionType", CryptoTransactionType.Token.ToString() },
				{ "TransactionDirection", TransactionDirection.Game.ToString() },
				{ "TransactionHash", transactionHash}
			};

			return await AzureService.TryMakeTransaction(requestBody);
		}

		public static async Task<string> TransferTokenToUsersCryptoWallet(VirtualCurrencyID virtualCurrencyID, int amount)
		{
			if (!IsWalletConnectReady())
			{
				Debug.LogError("WalletConnect is not ready.");
				return null;
			}

			string walletAddress = WalletConnect.GetActiveAccountAddress();

			if (walletAddress == null)
			{
				Debug.LogError("WalletConnect active account is not ready.");
				return null;
			}

			var usersWallet = WalletConnect.GetActiveAccountAddress();

			var requestBody = new JObject
			{
				{ "AuthContext", JsonConvert.SerializeObject(PlayFabAuthService.AuthContext) },
				{ "TransactionType", CryptoTransactionType.Token.ToString() },
				{ "TransactionDirection", TransactionDirection.UsersCryptoWallet.ToString() },
				{ "VirtualCurrencyID", virtualCurrencyID.ToString() },
				{ "Amount", amount },
				{ "WalletAddress", usersWallet }
			};

			return await AzureService.TryMakeTransaction(requestBody);
		}

		public static async Task<decimal> GetTokenBalance(VirtualCurrencyID virtualCurrencyID)
		{
			if (!IsWalletConnectReady())
			{
				Debug.LogError("WalletConnect is not ready.");
				return 0;
			}

			string walletAddress = WalletConnect.GetActiveAccountAddress();

			if (walletAddress == null)
			{
				Debug.LogError("WalletConnect active account is not ready.");
				return 0;
			}

			return await WalletConnectBlockchainService.GetTokenBalance(walletAddress, virtualCurrencyID);
		}

		public static async Task<decimal> GetNativeTokenBalance()
		{
			if (!IsWalletConnectReady())
			{
				Debug.LogError("WalletConnect is not ready.");
				return 0;
			}

			string walletAddress = WalletConnect.GetActiveAccountAddress();

			if (walletAddress == null)
			{
				Debug.LogError("WalletConnect active account is not ready.");
				return 0;
			}

			return await WalletConnectBlockchainService.GetNativeTokenBalance(walletAddress);
		}

		private static bool IsWalletConnectReady()
		{
			if (WalletConnect.ActiveSession == null)
			{
				return false;
			}

			if (WalletConnect.ActiveSession.Accounts.Length < 1)
			{
				return false;
			}

			return true;
		}
	}
}