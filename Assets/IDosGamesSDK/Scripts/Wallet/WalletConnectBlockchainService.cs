using IDosGames;
using Nethereum.Util;
using Nethereum.Web3;
using System.Numerics;
using System.Threading.Tasks;
using UnityEngine;
using WalletConnectSharp.Core.Models.Ethereum;
using WalletConnectSharp.Unity;

public static class WalletConnectBlockchainService
{
	public static async Task<decimal> GetTokenBalance(string walletAddress, VirtualCurrencyID virtualCurrencyID)
	{
		try
		{
			var web3 = new Web3(BlockchainSettings.ProviderAddress);
			string contractABI = GetTokenContractABI(virtualCurrencyID);
			string contractAddress = GetTokenContractAddress(virtualCurrencyID);
			var contract = web3.Eth.GetContract(contractABI, contractAddress);
			string balanceOfFunctionSignature = BlockchainSettings.BALANCE_OF_FUNCTION_SIGNATURE;
			var balanceOfFunction = contract.GetFunction(balanceOfFunctionSignature);
			var balanceResult = await balanceOfFunction.CallAsync<BigInteger>(walletAddress);

			return Web3.Convert.FromWei(balanceResult);
		}
		catch
		{
			Debug.LogError("Get BalanceOf Error");
			return 0;
		}
	}

	public static async Task<decimal> GetNativeTokenBalance(string walletAddress)
	{
		try
		{
			var web3 = new Web3(BlockchainSettings.ProviderAddress);
			var balanceResult = await web3.Eth.GetBalance.SendRequestAsync(walletAddress);

			return Web3.Convert.FromWei(balanceResult);
		}
		catch
		{
			return 0;
		}
	}

	public static async Task<string> TransferTokenAndGetHash(string fromAddress, string toAddress, VirtualCurrencyID tokenID, int amount)
	{
		var contractAddress = GetTokenContractAddress(tokenID);
		var contractABI = GetTokenContractABI(tokenID);
		var tokenAmount = Web3.Convert.ToWei(amount);

		var web3 = new Web3();
		var contract = web3.Eth.GetContract(contractABI, contractAddress);
		var transferFunction = contract.GetFunction(BlockchainSettings.TRANSFER_FUNCTION_SIGNATURE);
		var transferData = transferFunction.GetData(toAddress, tokenAmount);

		var transaction = new TransactionData
		{
			from = fromAddress,
			to = contractAddress,
			data = transferData,
			chainId = BlockchainSettings.ChainID
		};

		var result = await WalletConnect.ActiveSession.EthSendTransaction(transaction);

		return result;
	}

	public static string GetTokenContractAddress(VirtualCurrencyID tokenID)
	{
		string contractAddress = string.Empty;

		switch (tokenID)
		{
			case VirtualCurrencyID.IG:
				contractAddress = BlockchainSettings.IGT_CONTRACT_ADDRESS;
				break;
			case VirtualCurrencyID.CO:
				contractAddress = BlockchainSettings.IGC_CONTRACT_ADDRESS;
				break;
		}

		return contractAddress;
	}

	public static string GetTokenContractABI(VirtualCurrencyID tokenID)
	{
		string contractAddress = string.Empty;

		switch (tokenID)
		{
			case VirtualCurrencyID.IG:
				contractAddress = BlockchainSettings.IGT_CONTRACT_ABI;
				break;
			case VirtualCurrencyID.CO:
				contractAddress = BlockchainSettings.IGC_CONTRACT_ABI;
				break;
		}

		return contractAddress;
	}
}
