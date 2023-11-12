using System.Collections;
using UnityEngine;

#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif

namespace IDosGames
{
	public class AdBanner : MonoBehaviour
	{
		private const int SHOW_DELAY = 5;
		private AdMediation _adMediation => AdMediation.Instance;

		private void Start()
		{
			StartCoroutine(TryShowBanner());
		}

		private IEnumerator TryShowBanner()
		{
			yield return new WaitForSecondsRealtime(SHOW_DELAY);

#if UNITY_IOS
        while (!GetIOSAppTrackingTransparencyStatus())
        {
            yield return new WaitForSecondsRealtime(SHOW_DELAY);
        }
#endif

			if (!UserInventory.HasVIPStatus)
			{
				if (_adMediation != null)
				{
					_adMediation.ShowBanner();
				}
			}
		}

#if UNITY_IOS
    private bool GetIOSAppTrackingTransparencyStatus()
    {
        var status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();

        return status != ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED;
    }
#endif
	}
}