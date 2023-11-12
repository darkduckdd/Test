using Firebase.DynamicLinks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IDosGames
{
	public class ReferralSystem : MonoBehaviour
	{
		public static string ReferralLink { get; private set; }

		[SerializeField] private InviteFriendsPopUp _popUp;

		private static string _referralLinkDescription = "Install the app from this link and get rewarded from me: ";

		private void OnEnable()
		{
			DynamicLinks.DynamicLinkReceived += OnDynamicLink;
			PlayFabDataService.AllDataUpdated += _popUp.ResetView;
		}

		private void OnDisable()
		{
			DynamicLinks.DynamicLinkReceived -= OnDynamicLink;
			PlayFabDataService.AllDataUpdated -= _popUp.ResetView;
		}

		private void Start()
		{
			CreateReferralDynamicLink();
		}

		private void OnDynamicLink(object sender, EventArgs args)
		{
			var dynamicLinkEventArgs = args as ReceivedDynamicLinkEventArgs;
			Debug.LogFormat("Received dynamic link {0}", dynamicLinkEventArgs.ReceivedDynamicLink.Url.OriginalString);

			string link = dynamicLinkEventArgs.ReceivedDynamicLink.Url.OriginalString;

			string playfabID = link.Split('=').Last();

			if (playfabID != string.Empty)
			{
				ActivateReferralCode(playfabID);
			}
		}

		public void ActivateReferralCode(string code)
		{
			var parameter = new Dictionary<string, object>()
			{
				{
					PlayFabCloudFunctionParameter.REFERRAL_CODE, code
				}
			};

			PlayFabService.ExecuteCloudFunction(PlayFabCloudFunctionName.REFERRAL_ACTIVATE_REFERRAL_CODE, resultCallback: OnActivateResultCallback, errorCallback: OnActivateErrorCallback, functionParameter: parameter);
		}

		private void OnActivateResultCallback(ExecuteCloudScriptResult result)
		{
			if (result != null && result.FunctionResult != null)
			{
				JObject json = JsonConvert.DeserializeObject<JObject>(result.FunctionResult.ToString());

				if (json != null)
				{
					var message = json[PlayFabReturnMessage.KEY_MESSAGE].ToString();

					Message.Show(message);

					if (message == PlayFabReturnMessage.REFERRAL_MESSAGE_CODE_SUCCESS_ACTIVATED || message == PlayFabReturnMessage.REFERRAL_MESSAGE_CODE_SUCCESS_CHANGED)
					{
						_popUp.OnSuccessActivated();
					}
				}
			}
			else
			{
				Message.Show("Something went wrong");
			}
		}

		private void OnActivateErrorCallback(PlayFabError error)
		{
			Message.Show("Something went wrong");
		}

		private void CreateReferralDynamicLink()
		{
			var baseLink = IDosGamesSDKSettings.Instance.FirebaseBaseDynamicLink;
			var uriPrefix = IDosGamesSDKSettings.Instance.FirebaseDynamicLinkURIPrefix;

			var iosParameters = new IOSParameters(IDosGamesSDKSettings.Instance.IosBundleID)
			{
				AppStoreId = IDosGamesSDKSettings.Instance.IosAppStoreID
			};

			var components = new DynamicLinkComponents(

			new Uri(baseLink + "?Referral_ID=" + PlayFabAuthService.PlayFabID),
					uriPrefix)
			{
				IOSParameters = iosParameters,
				AndroidParameters = new AndroidParameters(IDosGamesSDKSettings.Instance.AndroidBundleID),
			};

			ReferralLink = components.LongDynamicLink.ToString();

			var options = new DynamicLinkOptions
			{
				PathLength = DynamicLinkPathLength.Unguessable
			};

			DynamicLinks.GetShortLinkAsync(components, options).ContinueWith(task =>
			{
				if (task.IsCanceled)
				{
					Debug.LogError("GetShortLinkAsync was canceled.");
					return;
				}
				if (task.IsFaulted)
				{
					Debug.LogError("GetShortLinkAsync encountered an error: " + task.Exception);
					return;
				}

				ShortDynamicLink link = task.Result;

				ReferralLink = link.Url.ToString();
			});
		}

		public static void Share()
		{
			new NativeShare().SetSubject(Application.productName)
				.SetText(_referralLinkDescription).SetUrl(ReferralLink)
				.SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
				.Share();
		}
	}
}