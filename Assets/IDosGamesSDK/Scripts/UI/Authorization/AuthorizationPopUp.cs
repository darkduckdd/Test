using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IDosGames
{
	public class AuthorizationPopUp : PopUp
	{
		[SerializeField] private TMP_InputField _emailInputField;
		[SerializeField] private TMP_InputField _passwordInputField;
		[SerializeField] private TMP_InputField _passwordRepeatInputField;
		[SerializeField] private Button _logInButton;
		[SerializeField] private Button _signUpButton;

		private void Start()
		{
			ResetLogInButton();
			ResetSignUpButton();
		}

		private void ResetLogInButton()
		{
			_logInButton.onClick.RemoveAllListeners();
			_logInButton.onClick.AddListener(TryLogIn);
		}

		private void ResetSignUpButton()
		{
			_signUpButton.onClick.RemoveAllListeners();
			_signUpButton.onClick.AddListener(TrySignUp);
		}

		private void TryLogIn()
		{
			var isEmailInputCorrect = CheckEmailInput();
			var isPasswordInputCorrect = CheckPasswordLength();

			if (isEmailInputCorrect && isPasswordInputCorrect)
			{
				LogIn();
			}
			else
			{
				ShowErrorMessage(isEmailInputCorrect, isPasswordInputCorrect);
			}
		}

		private void LogIn()
		{
			PlayFabAuthService.Instance.LoginWithEmailAddress(GetEmailInput(), GetPasswordInput(), OnLogInSuccess, PlayFabAuthService.ShowErrorMessage);
		}

		private void TrySignUp()
		{
			var isEmailInputCorrect = CheckEmailInput();
			var isPasswordInputCorrect = CheckPasswordLength();
			var isPasswordsMatch = CheckPasswordsMatch();

			if (isEmailInputCorrect && isPasswordInputCorrect && isPasswordsMatch)
			{
				if (PlayFabAuthService.AuthContext == null)
				{
					Message.Show("Must be logged in to call this method");
					return;
				}

				SignUp();
			}
			else
			{
				ShowErrorMessage(isEmailInputCorrect, isPasswordInputCorrect, isPasswordsMatch);
			}
		}

		private void SignUp()
		{
			PlayFabAuthService.Instance.AddUsernamePassword(GetEmailInput(), GetPasswordInput(), OnSignUpSuccess, PlayFabAuthService.ShowErrorMessage);
		}

		private void ShowErrorMessage(bool isEmailInputCorrect, bool isPasswordInputCorrect, bool isPasswordsMatch = true)
		{
			string message = GenerateErrorMessage(isEmailInputCorrect, isPasswordInputCorrect, isPasswordsMatch);
			Message.Show(message);
		}

		private string GenerateErrorMessage(bool isEmailInputCorrect, bool isPasswordInputCorrect, bool isPasswordsMatch = true)
		{
			string message = string.Empty;

			if (isEmailInputCorrect == false)
			{
				message = "Email address is not correct.";
			}
			else if (isPasswordInputCorrect == false)
			{
				message = "Password length must be between 6 and 100 characters.";
			}
			else if (isPasswordsMatch == false)
			{
				message = "Password mismatch.";
			}

			return message;
		}

		private bool CheckPasswordLength()
		{
			var input = GetPasswordInput();

			return PlayFabAuthService.CheckPasswordLenght(input);
		}

		private bool CheckPasswordsMatch()
		{
			var input1 = GetPasswordInput();
			var input2 = GetRepeatPasswordInput();

			bool isPasswordsMatch = input1 == input2;

			return isPasswordsMatch;
		}

		private bool CheckEmailInput()
		{
			var input = GetEmailInput();
			return PlayFabAuthService.CheckEmailAddress(input);
		}

		private void OnLogInSuccess(LoginResult result)
		{
			Message.Show("Success logged In.");
			PlayFabDataService.RequestAllData();
		}

		private void OnSignUpSuccess(AddUsernamePasswordResult result)
		{
			Message.Show("You have successfully linked your account to the specified email address");
		}

		public string GetEmailInput()
		{
			return _emailInputField.text;
		}

		private string GetPasswordInput()
		{
			return _passwordInputField.text;
		}

		private string GetRepeatPasswordInput()
		{
			return _passwordRepeatInputField.text;
		}
	}
}