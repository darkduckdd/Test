using UnityEngine;

namespace IDosGames
{
	public class URLOpener : MonoBehaviour
	{
		private const string URL_PRIVACY_POLICY = "https://idosgames.com/privacy/";
		private const string URL_TERMS_OF_USE = "https://idosgames.com/terms/";
		private const string URL_CRYPTOCURRENCY_SWAP = "https://idos.games";

		public static void OpenPrivacyPolicy()
		{
			Application.OpenURL(URL_PRIVACY_POLICY);
		}

		public static void OpenTermsofUse()
		{
			Application.OpenURL(URL_TERMS_OF_USE);
		}

		public static void OpenCryptocurrencySwap()
		{
			Application.OpenURL(URL_CRYPTOCURRENCY_SWAP);
		}
	}
}