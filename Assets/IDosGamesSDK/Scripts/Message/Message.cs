using PlayFab;
using System;
using System.Collections;
using UnityEngine;
using WalletConnectSharp.Unity;

namespace IDosGames
{
	public class Message : MonoBehaviour
	{
		private const int MAX_MESSAGE_LENGTH = 200;
		private const string CONTACT_TO_TECHNICAL_SUPPORT_TEXT = "Contact to technical support:";
		[SerializeField] private MessagePopUp _messagePopUp;
		[SerializeField] private RewardPopUp _rewardPopUp;
		[SerializeField] private ConnectionErrorPopUp _connectionErrorPopUp;

		private const int SHOW_CONNECTION_ERROR_POPUP_DELAY = 2;

		private static Message _instance;

		public static event Action Showed;

		private void Awake()
		{
			if (_instance == null || ReferenceEquals(this, _instance))
			{
				_instance = this;
				DontDestroyOnLoad(gameObject);
			}

			HideAllPopUps();
		}

		private void OnEnable()
		{
			PlayFabService.ConnectionError += StartDelayShowConnectionError;
			PlayFabDataService.AllDataRequestError += OnAllDataRequestError;
			WalletConnect.ActiveSessionNotReady += ShowConnectionError;
		}

		private void OnDisable()
		{
			PlayFabService.ConnectionError -= StartDelayShowConnectionError;
			PlayFabDataService.AllDataRequestError -= OnAllDataRequestError;
			WalletConnect.ActiveSessionNotReady -= ShowConnectionError;
		}

		public static void Show(string message)
		{
			if (_instance == null)
			{
				return;
			}

			if (message == null)
			{
				return;
			}

			if (message.Length > MAX_MESSAGE_LENGTH)
			{
				message = message[..MAX_MESSAGE_LENGTH];
			}

			_instance._messagePopUp.Set(message);
			_instance.ShowPopUp(_instance._messagePopUp);
		}

		public static void ShowReward(string message, string imagePath)
		{
			if (_instance == null)
			{
				return;
			}

			_instance._rewardPopUp.Set(message, imagePath);
			_instance.ShowPopUp(_instance._rewardPopUp);
		}

		private static void StartDelayShowConnectionError(Action callbackAction)
		{
			if (_instance == null)
			{
				return;
			}

			_instance.StartCoroutine(_instance.ShowDelayedConnectionError(callbackAction));
		}

		private IEnumerator ShowDelayedConnectionError(Action callbackAction)
		{
			yield return new WaitForSeconds(SHOW_CONNECTION_ERROR_POPUP_DELAY);
			ShowConnectionError(callbackAction);
		}

		private static void ShowConnectionError(Action callbackAction)
		{
			if (_instance == null)
			{
				return;
			}



			_instance._connectionErrorPopUp.Set(callbackAction);
			_instance.ShowPopUp(_instance._connectionErrorPopUp);
		}

		private void ShowPopUp(PopUp popUp)
		{
			popUp.gameObject.SetActive(true);

			Showed?.Invoke();
		}
		private void HideAllPopUps()
		{
			if (_instance == null)
			{
				return;
			}

			_instance._messagePopUp.gameObject.SetActive(false);
			_instance._rewardPopUp.gameObject.SetActive(false);
			_instance._connectionErrorPopUp.gameObject.SetActive(false);
		}

		private void OnAllDataRequestError(PlayFabError error)
		{
			Show($"<color=red><b>{CONTACT_TO_TECHNICAL_SUPPORT_TEXT}</b></color> \n\n" + new string(error.ErrorMessage));
		}
	}
}