#if IDOSGAMES_AD_MEDIATION_IRONSOURCE

using System;
using UnityEngine;

namespace IDosGames
{
	public class IronSourceMediation : MonoBehaviour, IAdMediation
	{
#if UNITY_IOS
		private string APP_KEY => IDosGamesSDKSettings.Instance.MediationAppKeyIOS;
#else
		private string APP_KEY => IDosGamesSDKSettings.Instance.MediationAppKeyAndroid;
#endif
		private BannerPosition _bannerPosition => IDosGamesSDKSettings.Instance.BannerPosition;

		private bool _bannerEnabled => IDosGamesSDKSettings.Instance.BannerEnabled;

		private Action<bool> _onRewardedVideoClosed;

		public event Action<AdRevenueData> AdRevenueDataReceived;

		[Obsolete]
		private void OnEnable()
		{
			SetCallbacks();
		}

		private void OnApplicationPause(bool isPaused)
		{
			IronSource.Agent.onApplicationPause(isPaused);
		}

		public void Initialize()
		{
			IronSource.Agent.validateIntegration();
			IronSource.Agent.shouldTrackNetworkState(true);
			IronSource.Agent.init(APP_KEY, IronSourceAdUnits.REWARDED_VIDEO, IronSourceAdUnits.INTERSTITIAL, IronSourceAdUnits.BANNER);

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
			if (APP_KEY != null && APP_KEY != string.Empty)
			{
				IronSourceAdQuality.Initialize(APP_KEY);
				IronSourceAdQuality.ChangeUserId(PlayFabAuthService.PlayFabID);
			}
#endif

			LoadInterstitial();
		}

		[Obsolete]
		private void SetCallbacks()
		{
			IronSourceEvents.onSdkInitializationCompletedEvent += OnInitializationFinished;
			IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoClosed;
			IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
			IronSourceEvents.onImpressionDataReadyEvent += ImpressionDataReadyEvent;
		}

		private void ImpressionDataReadyEvent(IronSourceImpressionData impressionData)
		{
			if (impressionData == null)
			{
				return;
			}

			AdRevenueData revenue = new()
			{
				AdPlatform = "ironSource",
				AdType = impressionData.adUnit,
				AdNetwork = impressionData.adNetwork,
				AdUnitName = impressionData.instanceName,
				Currency = "USD",
				Revenue = impressionData.revenue
			};

			AdRevenueDataReceived?.Invoke(revenue);
		}

		private void OnInitializationFinished()
		{
			Debug.Log("IronSource Initialization Finished.");
		}

		public void ShowBanner()
		{
			if (_bannerEnabled)
			{
				LoadBanner();
			}
		}

		private void LoadBanner()
		{
			switch (_bannerPosition)
			{
				case BannerPosition.Top:
					IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.TOP);
					break;

				case BannerPosition.Bottom:
					IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
					break;

				default:
					IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
					break;
			}
		}

		public void HideBanner()
		{
			IronSource.Agent.destroyBanner();
		}

		public void RemoveAds()
		{
			HideBanner();
		}

		private void LoadInterstitial()
		{
			IronSource.Agent.loadInterstitial();
		}

		public bool ShowInterstitialAd()
		{
			if (IronSource.Agent.isInterstitialReady())
			{
				IronSource.Agent.showInterstitial();
				return true;
			}
			else
			{
				IronSource.Agent.loadInterstitial();

				if (IronSource.Agent.isInterstitialReady())
				{
					ShowInterstitialAd();
				}
			}
			return false;
		}

		public bool ShowRewardedVideo(Action<bool> onRewardedVideoClosed)
		{
			if (IronSource.Agent.isRewardedVideoAvailable())
			{
				_onRewardedVideoClosed = onRewardedVideoClosed;
				IronSource.Agent.showRewardedVideo();
				return true;
			}

			return false;
		}

		private void RewardedVideoClosed()
		{
			_onRewardedVideoClosed.Invoke(true);
			_onRewardedVideoClosed = null;
		}

		private void RewardedVideoAdShowFailedEvent(IronSourceError error)
		{
			_onRewardedVideoClosed.Invoke(false);
			_onRewardedVideoClosed = null;
		}

		public void SetConsent(bool consent)
		{
			IronSource.Agent.setConsent(consent);
		}
	}

}
#endif