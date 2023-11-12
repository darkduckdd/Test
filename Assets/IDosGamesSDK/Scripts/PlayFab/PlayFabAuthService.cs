using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace IDosGames
{
	public class PlayFabAuthService
	{
		private const int PLAYFAB_PASSWORD_MIN_LENGTH = 6;
		private const int PLAYFAB_PASSWORD_MAX_LENGTH = 100;
		private const string EMAIL_REGEX = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";

		private const string SAVED_AUTH_TYPE_KEY = "Saved_AuthType";
		private const string SAVED_AUTH_EMAIL_KEY = "Saved_Auth_Email";
		private const string SAVED_AUTH_PASSWORD_KEY = "Saved_Auth_Password";

		public static AuthType LastAuthType => (AuthType)PlayerPrefs.GetInt(SAVED_AUTH_TYPE_KEY, (int)AuthType.None);
		public static bool IsLoggedIn => LastAuthType != AuthType.Device && LastAuthType != AuthType.None;
		public static string SavedEmail => PlayerPrefs.GetString(SAVED_AUTH_EMAIL_KEY, string.Empty);
		public static string SavedPassword => PlayerPrefs.GetString(SAVED_AUTH_PASSWORD_KEY, string.Empty);

		public static string PlayFabID { get; private set; }
		public static PlayFabAuthenticationContext AuthContext { get; private set; }

		private static PlayFabAuthService _instance;

		public static event Action RequestSent;

		public static event Action LoggedIn;

		public static PlayFabAuthService Instance => _instance;

		public PlayFabAuthService()
		{
			_instance = this;
		}

		[RuntimeInitializeOnLoadMethod]
		private static void Initialize()
		{
			_instance = new();
		}

		public void LoginWithDeviceID(Action<LoginResult> resultCallback = null, Action<PlayFabError> errorCallback = null, Action retryCallback = null)
		{
			RequestSent?.Invoke();

#if UNITY_ANDROID
			PlayFabClientAPI.LoginWithAndroidDeviceID(new LoginWithAndroidDeviceIDRequest()
			{
				AndroidDeviceId = SystemInfo.deviceUniqueIdentifier,
				AndroidDevice = SystemInfo.deviceModel,
				OS = SystemInfo.operatingSystem,
				CreateAccount = true
			},
#elif UNITY_IOS
        PlayFabClientAPI.LoginWithIOSDeviceID(new LoginWithIOSDeviceIDRequest()
        {
            DeviceId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        },
#else
        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
        {
            CustomId = $"{Application.platform}_{SystemInfo.deviceUniqueIdentifier}",
            CreateAccount = true
        },
#endif
		result =>
		{
			SetCredentials(result);
			SaveAuthType(AuthType.Device);
			ClearEmailAndPassword();
			resultCallback?.Invoke(result);

			LoggedIn?.Invoke();
		},
			error => PlayFabService.OnPlayFabError(error, errorCallback, retryCallback)
			);
		}

		public void LogOut(Action<LoginResult> resultCallback = null, Action<PlayFabError> errorCallback = null, Action retryCallback = null)
		{
			LoginWithDeviceID(resultCallback, errorCallback, retryCallback);
		}

		public void LoginWithEmailAddress(string email, string password, Action<LoginResult> resultCallback = null, Action<PlayFabError> errorCallback = null, Action retryConnectionCallback = null)
		{
			RequestSent?.Invoke();

			PlayFabClientAPI.LoginWithEmailAddress(new LoginWithEmailAddressRequest()
			{
				Email = email,
				Password = password,
				InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
				{
					GetUserAccountInfo = true
				}
			},
			result =>
			{
				SetCredentials(result);
				SaveAuthType(AuthType.Email);
				SaveEmailAndPassword(email, password);
				resultCallback?.Invoke(result);

				LoggedIn?.Invoke();
			},
			error => PlayFabService.OnPlayFabError(error, errorCallback, retryConnectionCallback)
			);
		}

		public void AddUsernamePassword(string email, string password, Action<AddUsernamePasswordResult> resultCallback = null, Action<PlayFabError> errorCallback = null)
		{
			RequestSent?.Invoke();

			PlayFabClientAPI.AddUsernamePassword(new AddUsernamePasswordRequest()
			{
				Email = email,
				Username = PlayFabID,
				Password = password
			},
			result =>
			{

				SaveAuthType(AuthType.Email);
				SaveEmailAndPassword(email, password);
				resultCallback?.Invoke(result);

				LoggedIn?.Invoke();

			},
			error => PlayFabService.OnPlayFabError(error, errorCallback)
			);
		}

		public void SendAccountRecoveryEmail(string email, Action<SendAccountRecoveryEmailResult> resultCallback = null, Action<PlayFabError> errorCallback = null)
		{
			RequestSent?.Invoke();

			PlayFabClientAPI.SendAccountRecoveryEmail(new SendAccountRecoveryEmailRequest()
			{
				TitleId = PlayFabSettings.TitleId,
				Email = email
			},
			result => resultCallback?.Invoke(result),
			error => PlayFabService.OnPlayFabError(error, errorCallback)
			);
		}

		public void DeleteTitlePlayerAccount(Action resultCallback = null)
		{
			PlayFabService.ExecuteCloudFunction(PlayFabCloudFunctionName.DELETE_TITLE_PLAYER_ACCOUNT, (result) => resultCallback?.Invoke(), ShowErrorMessage);
		}

		private void SetCredentials(LoginResult result)
		{
			PlayFabID = result.PlayFabId.ToUpper();
			AuthContext = result.AuthenticationContext;
		}

		private void SaveAuthType(AuthType authType)
		{
			PlayerPrefs.SetInt(SAVED_AUTH_TYPE_KEY, (int)authType);
			PlayerPrefs.Save();
		}

		private void SaveEmailAndPassword(string email, string password)
		{
			PlayerPrefs.SetString(SAVED_AUTH_EMAIL_KEY, email);
			PlayerPrefs.SetString(SAVED_AUTH_PASSWORD_KEY, password);
			PlayerPrefs.Save();
		}

		private void ClearEmailAndPassword()
		{
			PlayerPrefs.SetString(SAVED_AUTH_EMAIL_KEY, string.Empty);
			PlayerPrefs.SetString(SAVED_AUTH_PASSWORD_KEY, string.Empty);
			PlayerPrefs.Save();
		}

		public static void ShowErrorMessage(PlayFabError error)
		{
			var message = GenerateErrorMessage(error);
			Message.Show(message);
		}

		private static string GenerateErrorMessage(PlayFabError error)
		{
			string message;

			switch (error.Error)
			{
				case PlayFabErrorCode.AccountNotFound:
					message = "Account not found. You can sign up.";
					break;

				case PlayFabErrorCode.InvalidEmailAddress:
					message = "Email address not exists.";
					break;

				case PlayFabErrorCode.InvalidPassword:
					message = "Password is not correct.";
					break;

				case PlayFabErrorCode.InvalidEmailOrPassword:
					message = "Password or email is not correct";
					break;

				case PlayFabErrorCode.AccountAlreadyLinked:
					message = "This account already linked to this or other email";
					break;

				case PlayFabErrorCode.AccountBanned:
					message = "This account banned";
					break;

				case PlayFabErrorCode.AccountDeleted:
					message = "This account deleted";
					break;

				case PlayFabErrorCode.AccountNotLinked:
					message = "This account not linked";
					break;

				case PlayFabErrorCode.EmailAddressNotAvailable:
					message = "This email is not available, already in use by another player";
					break;

				case PlayFabErrorCode.InvalidParams:
					message = "Invalid input parameters.";
					break;

				case PlayFabErrorCode.EmailRecipientBlacklisted:
					message = "Email Recipient Black listed.";
					break;

				default:
					message = $"Error code: {error.Error} | message: {error.ErrorMessage}";
					break;
			}

			return message;
		}

		public static bool CheckEmailAddress(string email)
		{
			return Regex.IsMatch(email, EMAIL_REGEX, RegexOptions.IgnoreCase);
		}

		public static bool CheckPasswordLenght(string password)
		{
			var lenght = password.Length;
			return lenght >= PLAYFAB_PASSWORD_MIN_LENGTH && lenght <= PLAYFAB_PASSWORD_MAX_LENGTH;
		}
	}
}