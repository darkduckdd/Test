using TMPro;
using UnityEngine;

namespace IDosGames
{
	public class AccountInfoView : MonoBehaviour
	{
		[SerializeField] private TMP_Text _playfabID;
		[SerializeField] private TMP_Text _email;

		private void OnEnable()
		{
			UpdateView();

			PlayFabAuthService.LoggedIn += UpdateView;
		}

		private void OnDisable()
		{
			PlayFabAuthService.LoggedIn -= UpdateView;
		}

		private void UpdateView()
		{
			SetPlayFabIDText();
			SetEmailText();
		}

		private void SetPlayFabIDText()
		{
			_playfabID.text = PlayFabAuthService.PlayFabID;
		}

		private void SetEmailText()
		{
			string email = PlayFabAuthService.SavedEmail;
			string haveNotLinkedText = "You need to login with email.";

			_email.text = email != string.Empty ? email : haveNotLinkedText;

		}
	}
}