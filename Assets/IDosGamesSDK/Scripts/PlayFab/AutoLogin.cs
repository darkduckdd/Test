using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace IDosGames
{
	public class AutoLogin : MonoBehaviour
	{
		private AuthType _lastAuthType => PlayFabAuthService.LastAuthType;
		private string _savedEmail => PlayFabAuthService.SavedEmail;
		private string _savedPassword => PlayFabAuthService.SavedPassword;

		private void Start()
		{
			Login();
		}

		public void Login()
		{
			switch (_lastAuthType)
			{
				case AuthType.Email:
					AutoLoginWithEmail();
					break;

				default:
					AutoLoginWithDeviceID();
					break;
			}
		}

		private void AutoLoginWithEmail()
		{
			PlayFabAuthService.Instance.LoginWithEmailAddress(_savedEmail, _savedPassword, OnSuccessAutoLogin, OnErrorAutoLogin, OnRetryAutoLogin);
		}

		private void AutoLoginWithDeviceID()
		{
			PlayFabAuthService.Instance.LoginWithDeviceID(OnSuccessAutoLogin, OnErrorAutoLogin, OnRetryAutoLogin);
		}

		private void OnSuccessAutoLogin(LoginResult result)
		{
			Loading.SwitchToNextScene();
		}

		private void OnRetryAutoLogin()
		{
			Login();
		}

		private void OnErrorAutoLogin(PlayFabError error)
		{
			Debug.Log("Auto login Error: " + error);
			AutoLoginWithDeviceID();
		}
	}
}