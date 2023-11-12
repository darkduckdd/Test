using System;
using UnityEngine;
using UnityEngine.UI;

namespace IDosGames
{
	public class PopUpWalletTransfer : MonoBehaviour
	{
		[SerializeField] private WalletWindow _walletWindow;
		[SerializeField] private PopUpWalletTransferView _view;
		[SerializeField] private Button _transferButton;
		[SerializeField] private PopUpTransferConfirmation _confirmationPopUp;

		private void Start()
		{
			_transferButton.onClick.RemoveAllListeners();
			_transferButton.onClick.AddListener(OnClickTransfer);
		}

		public void TryTransferToken()
		{
			var amountInput = _view.GetAmountInput();
			var virtualCurrencyID = _view.GetTokenInput();
			var direction = _view.GetTransferDirection();

			int.TryParse(amountInput, out int amount);

			_walletWindow.TransferToken(direction, virtualCurrencyID, amount);
		}

		private void OnClickTransfer()
		{
			var amountInput = _view.GetAmountInput();

			if (amountInput == string.Empty)
			{
				Message.Show("Amount is a required field.");
				_view.ChangeAmountFieldColorToIncorrect();
				return;
			}

			bool isAmountCorrect = _view.GetAmountInputStatus();

			int.TryParse(amountInput, out int amount);

			if (!isAmountCorrect)
			{
				if (amount <= 0)
				{
					Message.Show("Amount must be greater than zero.");
				}
				else
				{
					Message.Show("You don't have enough resources.");
				}

				return;
			}

			if (_view.GetTransferDirection() == TransactionDirection.UsersCryptoWallet)
			{
				if (UserInventory.GetVirtualCurrencyAmount(VirtualCurrencyID.WK) < 1)
				{
					Message.Show("To transfer from the game you need to have 'Withdrawal Key'");
					ShopSystem.PopUpSystem.ShowWKPopUp();

					return;
				}
			}

			var tokenInput = _view.GetTokenInput();
			var tokenName = tokenInput == VirtualCurrencyID.IG ? JsonProperty.IGT.ToUpper() : JsonProperty.IGC.ToUpper();
			Sprite itemIcon = Resources.Load<Sprite>($"Sprites/Currency/{tokenName}");
			string itemDescription = $"<color=white>{tokenName}</color> {_view.GetTransferType()}";

			_confirmationPopUp.Set(OnClickConfirm, _view.GetTransferDirection(), itemIcon, amount, itemDescription);
			_confirmationPopUp.gameObject.SetActive(true);
		}

		private void OnClickConfirm()
		{
			TryTransferToken();
			gameObject.SetActive(false);
		}
	}
}