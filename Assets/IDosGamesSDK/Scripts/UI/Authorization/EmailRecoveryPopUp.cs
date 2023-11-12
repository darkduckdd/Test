using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IDosGames
{
	public class EmailRecoveryPopUp : PopUp
	{
		[SerializeField] private AuthorizationPopUpView _authorizationPopUpView;
		[SerializeField] private TMP_InputField _emailInputField;
		[SerializeField] private Button _sendButton;

		private void Start()
		{
			ResetSendButton();
		}

		private void ResetSendButton()
		{
			_sendButton.onClick.RemoveAllListeners();
			_sendButton.onClick.AddListener(TrySend);
		}

		private void TrySend()
		{
			bool isEmailInputCorrect = CheckEmailInput();

			if (isEmailInputCorrect)
			{
				Send();
			}
			else
			{
				ShowErrorMessage();
			}
		}

		private void ShowErrorMessage()
		{
			Message.Show("Email Address Is Not Correct.");
		}

		private void Send()
		{
			PlayFabAuthService.Instance.SendAccountRecoveryEmail(GetEmailInput(), OnSendSuccess, PlayFabAuthService.ShowErrorMessage);
		}

		private bool CheckEmailInput()
		{
			var email = GetEmailInput();
			return PlayFabAuthService.CheckEmailAddress(email);
		}

		private void OnSendSuccess(SendAccountRecoveryEmailResult result)
		{
			_authorizationPopUpView.CloseRecoveryPopUp();
			Message.Show("A password recovery message has been sent to your email.");
		}

		public void SetInputFieldText(string email)
		{
			_emailInputField.text = email;
		}

		public string GetEmailInput()
		{
			return _emailInputField.text;
		}
	}
}