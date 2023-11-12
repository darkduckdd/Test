using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

namespace IDosGames
{
	public class AuthorizationView : PopUp
	{
		[SerializeField] private AuthorizationPopUpView _popUp;
		[SerializeField] private Button _AuthorizationBtn;
		[SerializeField] private Button _logOutBtn;
		[SerializeField] private Button _deleteAccountButton;
		[SerializeField] private GameObject _popUpDeleteAccount;

		private void OnEnable()
		{
			UpdateView();
			PlayFabAuthService.LoggedIn += UpdateView;
		}

		private void OnDisable()
		{
			PlayFabAuthService.LoggedIn -= UpdateView;
		}

		private void Start()
		{
			ResetButtons();
		}

		private void ResetButtons()
		{
			_AuthorizationBtn.onClick.RemoveAllListeners();
			_AuthorizationBtn.onClick.AddListener(ActivateAuthorizationPopUp);

			_logOutBtn.onClick.RemoveAllListeners();
			_logOutBtn.onClick.AddListener(LogOut);

			_deleteAccountButton.onClick.RemoveAllListeners();
			_deleteAccountButton.onClick.AddListener(() => SetActiveDeleteAccountPopUp(true));
		}

		private void ActivateAuthorizationPopUp()
		{
			_popUp.gameObject.SetActive(true);
		}

		private void LogOut()
		{
			PlayFabAuthService.Instance.LogOut(OnLogOutSuccess);
		}

		private void OnLogOutSuccess(LoginResult obj)
		{
			Message.Show("Success log out.");
			PlayFabDataService.RequestAllData();
		}

		private void UpdateView()
		{
			_AuthorizationBtn.gameObject.SetActive(!PlayFabAuthService.IsLoggedIn);
			_logOutBtn.gameObject.SetActive(PlayFabAuthService.IsLoggedIn);
			_deleteAccountButton.gameObject.SetActive(IsActiveDeleteAccountButton());
		}

		private bool IsActiveDeleteAccountButton()
		{
#if UNITY_IOS
			return PlayFabAuthService.IsLoggedIn && IDosGamesSDKSettings.Instance.IOSAccountDeletionEnabled;
#elif UNITY_ANDROID
			return PlayFabAuthService.IsLoggedIn && IDosGamesSDKSettings.Instance.AndroidAccountDeletionEnabled;
#else
			return false;
#endif
		}

		public void DeleteTitlePlayerAccount()
		{
			PlayFabAuthService.Instance.DeleteTitlePlayerAccount(OnSuccessDeleteTitlePlayerAccount);
		}

		private void OnSuccessDeleteTitlePlayerAccount()
		{
			Message.Show("Account successfully deleted.");
			SetActiveDeleteAccountPopUp(false);
			PlayFabAuthService.Instance.LogOut((result) => PlayFabDataService.RequestAllData());
		}

		private void SetActiveDeleteAccountPopUp(bool active)
		{
			_popUpDeleteAccount.SetActive(active);
		}
	}
}