using PlayFab.ClientModels;
using UnityEngine;

namespace IDosGames
{
	public class SpinWindow : MonoBehaviour
	{
		[SerializeField] private SpinWheel _spinWheel;
		[SerializeField] private SpinWindowView _spinWindowView;

		private void Start()
		{
			_spinWindowView.ResetSpinButtonsListener(TryToSpin);
		}

		private void TryToSpin(SpinTicketType type)
		{
			string functionName = "";

			switch (type)
			{
				case SpinTicketType.Standard:
					functionName = PlayFabCloudFunctionName.SPIN_GET_STANDARD_REWARD;
					break;
				case SpinTicketType.Premium:
					functionName = PlayFabCloudFunctionName.SPIN_GET_PREMIUM_REWARD;
					break;
			}

			ExecuteSpinCloudFunction(functionName);
		}

		public void TryToFreeSpin(bool showAd)
		{
			if (showAd)
			{
				if (AdMediation.Instance.ShowRewardedVideo(GetFreeSpin))
				{
					Debug.Log("Show rewarded video.");
				}
				else
				{
					Message.Show("Ad is not ready.");
				}
			}
			else
			{
				GetFreeSpin();
			}
		}

		private void GetFreeSpin(bool adCompleted = true)
		{
			ExecuteSpinCloudFunction(PlayFabCloudFunctionName.SPIN_GET_FREE_REWARD);
		}

		private void ExecuteSpinCloudFunction(string functionName)
		{
			PlayFabService.ExecuteCloudFunction(functionName, OnSuccessResponseSpinResult);
		}

		private void OnSuccessResponseSpinResult(ExecuteCloudScriptResult result)
		{
			if (result != null && result.FunctionResult != null)
			{
				int.TryParse(result.FunctionResult.ToString(), out int targetIndex);

				_spinWheel.Spin(targetIndex);

				PlayFabDataService.RequestAllData();
				Loading.HideAllPanels();
			}
			else
			{
				Message.Show("Something went wrong. Ñontact technical support.");
			}
		}

		public void OpenFreeSpin()
		{
			ActivateMainWindow();
			_spinWindowView.ShowSpinView(SpinTicketType.Free);
		}

		public void OpenStandardSpin()
		{
			ActivateMainWindow();
			_spinWindowView.ShowSpinView(SpinTicketType.Standard);
		}

		private void ActivateMainWindow()
		{
			gameObject.SetActive(true);
		}
	}
}