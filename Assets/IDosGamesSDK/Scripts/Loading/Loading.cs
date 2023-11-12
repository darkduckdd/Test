using UnityEngine;

namespace IDosGames
{
	public class Loading : MonoBehaviour
	{
		[SerializeField] private LoadingPanel _panel;
		[SerializeField] private TouchBlocker _touchBlocker;
		[SerializeField] private SceneSwitcher _sceneSwitcher;

		private static Loading _instance;

		private void Awake()
		{
			if (_instance == null || ReferenceEquals(this, _instance))
			{
				_instance = this;
				DontDestroyOnLoad(gameObject);
			}
		}

		private void OnEnable()
		{
			PlayFabDataService.AllDataRequested += ShowTransparentPanel;
			PlayFabDataService.AllDataUpdated += HideTransparentPanel;
			PlayFabService.CloudFunctionCalled += ShowTransparentPanel;
			PlayFabService.CloudFunctionResponsed += HideTransparentPanel;
			PlayFabAuthService.RequestSent += ShowTransparentPanel;
			SceneSwitcher.SwitchSceneStarted += ShowOpaquePanel;
			SceneSwitcher.SwitchSceneFinished += HideOpaquePanel;
			Message.Showed += HideAllPanels;
		}

		private void OnDisable()
		{
			PlayFabDataService.AllDataRequested -= ShowTransparentPanel;
			PlayFabDataService.AllDataUpdated -= HideTransparentPanel;
			PlayFabService.CloudFunctionCalled -= ShowTransparentPanel;
			PlayFabService.CloudFunctionResponsed -= HideTransparentPanel;
			PlayFabAuthService.RequestSent -= ShowTransparentPanel;
			SceneSwitcher.SwitchSceneStarted -= ShowOpaquePanel;
			SceneSwitcher.SwitchSceneFinished -= HideOpaquePanel;
			Message.Showed -= HideAllPanels;
		}

		public static void ShowTransparentPanel()
		{
			if (_instance == null)
			{
				return;
			}

			_instance._panel.Show(LoadingPanelType.Transparent);
		}

		public static void ShowOpaquePanel()
		{
			if (_instance == null)
			{
				return;
			}

			_instance._panel.Show(LoadingPanelType.Opaque);
		}

		public static void HideAllPanels()
		{
			if (_instance == null)
			{
				return;
			}

			_instance._panel.HideAllPanels();
		}

		private static void HideOpaquePanel()
		{
			if (_instance == null)
			{
				return;
			}

			_instance._panel.HideOpaquePanel();
		}

		private static void HideTransparentPanel()
		{
			if (_instance == null)
			{
				return;
			}

			_instance._panel.HideTransparentPanel();
		}

		public static void SwitchToNextScene()
		{
			if (_instance == null)
			{
				return;
			}

			_instance._sceneSwitcher.SwitchToNextScene();
		}

		public static void SwitchToPreviousScene()
		{
			if (_instance == null)
			{
				return;
			}

			_instance._sceneSwitcher.SwitchToPreviousScene();
		}

		public static void BlockTouch()
		{
			if (_instance == null)
			{
				return;
			}

			_instance._touchBlocker.Block();
		}

		public static void UnblockTouch()
		{
			if (_instance == null)
			{
				return;
			}

			_instance._touchBlocker.Unblock();
		}
	}
}