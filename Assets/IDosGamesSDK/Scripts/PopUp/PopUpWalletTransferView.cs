using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IDosGames
{
	public class PopUpWalletTransferView : MonoBehaviour
	{
		[SerializeField] private TMP_Dropdown _tokenDropdown;
		[SerializeField] private AmountInputField _amountInputField;
		[SerializeField] private WalletTransferDirection _transferDirection;
		[SerializeField] private PanelCryptoWalletTokenBalance _cryptoTokenBalance;
		[SerializeField] private ButtonWithOptionalIcon _transferButton;

		private CryptoTransactionType _transactionType = CryptoTransactionType.Token;

		private void OnEnable()
		{
			ResetTokenDropdown();
			ResetAmountInputField();

			UpdateIconVisibilityOnTransferBtn();
			_transferDirection.ValueChanged += UpdateAvailableAmount;
			_transferDirection.ValueChanged += UpdateIconVisibilityOnTransferBtn;

			_tokenDropdown.onValueChanged.AddListener((text) => UpdateAvailableAmount());
		}

		private void OnDisable()
		{
			_transferDirection.ValueChanged -= UpdateAvailableAmount;
			_tokenDropdown.onValueChanged.RemoveAllListeners();
		}

		public string GetAmountInput()
		{
			return _amountInputField.GetInput().Trim();
		}

		public bool GetAmountInputStatus()
		{
			return _amountInputField.IsAmountCorrect;
		}

		public CryptoTransactionType GetTransferType()
		{
			return _transactionType;
		}

		public void ChangeAmountFieldColorToIncorrect()
		{
			_amountInputField.ChangeOuterFrameColor(false);
		}

		public VirtualCurrencyID GetTokenInput()
		{
			if (_tokenDropdown.captionText.text.Trim().ToUpper() == JsonProperty.IGT.ToUpper())
			{
				return VirtualCurrencyID.IG;
			}
			else
			{
				return VirtualCurrencyID.CO;
			}
		}

		public TransactionDirection GetTransferDirection()
		{
			return _transferDirection.Direction;
		}

		private void ResetTokenDropdown()
		{
			_tokenDropdown.value = 0;
		}

		private void ResetAmountInputField()
		{
			_amountInputField.ResetInput();
			UpdateAvailableAmount();
		}

		private void UpdateAvailableAmount()
		{
			int amount = 0;

			var direction = GetTransferDirection();
			var tokenInput = GetTokenInput();

			if (direction == TransactionDirection.UsersCryptoWallet)
			{
				amount = UserInventory.GetVirtualCurrencyAmount(tokenInput);
			}
			else if (direction == TransactionDirection.Game)
			{
				if (tokenInput == VirtualCurrencyID.IG)
				{
					amount = _cryptoTokenBalance.BalanceOfIGT;
				}
				else if (tokenInput == VirtualCurrencyID.CO)
				{
					amount = _cryptoTokenBalance.BalanceOfIGC;
				}
			}

			_amountInputField.UpdateAvailableAmount(amount);
		}

		private void UpdateIconVisibilityOnTransferBtn()
		{
			_transferButton.SetActivateIcon(GetTransferDirection() == TransactionDirection.UsersCryptoWallet);
		}
	}
}