using System;
using TMPro;
using UnityEngine;
using WalletConnectSharp.Unity;

namespace IDosGames
{
	public class PanelCryptoWalletTokenBalance : MonoBehaviour
	{
		public const string AMOUNT_LOADING_TEXT = "...";

		public const int TOKEN_DIGITS_AMOUNT_AFTER_DOT = 0;
		public readonly string TOKEN_AMOUNT_FORMAT = $"N{TOKEN_DIGITS_AMOUNT_AFTER_DOT}";
		public const int Native_TOKEN_DIGITS_AMOUNT_AFTER_DOT = 5;
		public readonly string NATIVE_TOKEN_AMOUNT_FORMAT = $"N{Native_TOKEN_DIGITS_AMOUNT_AFTER_DOT}";

		public int BalanceOfIGT { get; private set; }
		public int BalanceOfIGC { get; private set; }
		public decimal BalanceOfNativeToken { get; private set; }

		[SerializeField] private GameObject _loading;
		[SerializeField] private GameObject _buttonRefresh;
		[SerializeField] private TMP_Text _amountIGT;
		[SerializeField] private TMP_Text _amountIGC;
		[SerializeField] private TMP_Text _amountNativeToken;

		private void Start()
		{
			Refresh();
		}

		public async void Refresh()
		{
			SetActivateLoading(true);

			BalanceOfIGT = (int)await WalletService.GetTokenBalance(VirtualCurrencyID.IG);
			BalanceOfIGC = (int)await WalletService.GetTokenBalance(VirtualCurrencyID.CO);
			BalanceOfNativeToken = await WalletService.GetNativeTokenBalance();

			UpdateUI();
			SetActivateLoading(false);
		}

		private void SetActivateLoading(bool active)
		{
			_loading.SetActive(active);
			_buttonRefresh.SetActive(!active);

			if (active)
			{
				UpdateIGTAmountUI(AMOUNT_LOADING_TEXT);
				UpdateIGCAmountUI(AMOUNT_LOADING_TEXT);
				UpdateNativeTokenAmountUI(AMOUNT_LOADING_TEXT);
			}
		}

		private void UpdateUI()
		{
			UpdateIGTAmountUI(BalanceOfIGT.ToString(TOKEN_AMOUNT_FORMAT));
			UpdateIGCAmountUI(BalanceOfIGC.ToString(TOKEN_AMOUNT_FORMAT));
			UpdateNativeTokenAmountUI(GetNativeTokenAmountString());
		}

		private string GetNativeTokenAmountString()
		{
			if (BalanceOfNativeToken * (decimal)Math.Pow(10, Native_TOKEN_DIGITS_AMOUNT_AFTER_DOT) <= 0)
			{
				return "0";
			}

			return BalanceOfNativeToken.ToString(NATIVE_TOKEN_AMOUNT_FORMAT);
		}

		private void UpdateIGTAmountUI(string text)
		{
			_amountIGT.text = text;
		}

		private void UpdateIGCAmountUI(string text)
		{
			_amountIGC.text = text;
		}

		private void UpdateNativeTokenAmountUI(string text)
		{
			_amountNativeToken.text = text;
		}
	}
}