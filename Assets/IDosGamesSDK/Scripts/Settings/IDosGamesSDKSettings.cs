using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace IDosGames
{
	public class IDosGamesSDKSettings : ScriptableObject
	{
		private static IDosGamesSDKSettings _instance;

		public static IDosGamesSDKSettings Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = Resources.Load<IDosGamesSDKSettings>("Settings/IDosGamesSDKSettings");
				}
				return _instance;
			}
		}

		[Space(5)]
		[Header("App Settings")]
		[Space(5)]
		[SerializeField] private string _iosBundleID;
		public string IosBundleID => _iosBundleID;

		[SerializeField] private string _iosAppStoreID;
		public string IosAppStoreID => _iosAppStoreID;

		[Space(5)]

		[SerializeField] private string _androidBundleID;
		public string AndroidBundleID => _androidBundleID;

		[Space(5)]
		[Header("Ad Mediation")]
		[Space(5)]
		[SerializeField] private AdMediationPlatform _adMediationPlatform;

		private const string AD_MEDIATION_DEFINE_PREFIX = "IDOSGAMES_AD_MEDIATION_";

		private AdMediationPlatform _selectedAdMediationPlatform;

		[SerializeField] private string _mediationAppKeyIOS = "";

		public string MediationAppKeyIOS => _mediationAppKeyIOS;

		[SerializeField] private string _mediationAppKeyAndroid = "";
		public string MediationAppKeyAndroid => _mediationAppKeyAndroid;

		[SerializeField] private BannerPosition _banerPosition = BannerPosition.Bottom;
		public BannerPosition BannerPosition => _banerPosition;

		[SerializeField] private bool _bannerEnabled;
		public bool BannerEnabled => _bannerEnabled;

		[Space(5)]
		[Header("Analytics")]
		[Space(5)]
		[SerializeField] private string _appMetricaApiKey;
		public string AppMetricaApiKey => _appMetricaApiKey;

		[Header("Referral System")]
		[Space(5)]
		[SerializeField] private string _firebaseBaseDynamicLink = "https://idosgames.com/games/example/";
		public string FirebaseBaseDynamicLink => _firebaseBaseDynamicLink;

		[SerializeField] private string _firebaseDynamicLinkURIPrefix = "https://example.page.link";

		public string FirebaseDynamicLinkURIPrefix => _firebaseDynamicLinkURIPrefix;


		[Header("Account")]
		[Space(5)]
		[SerializeField] private bool _iOSAccountDeletionEnabled;
		public bool IOSAccountDeletionEnabled => _iOSAccountDeletionEnabled;

		[SerializeField] private bool _AndroidAccountDeletionEnabled;
		public bool AndroidAccountDeletionEnabled => _AndroidAccountDeletionEnabled;

		[Header("Wallet")]
		[Space(5)]
		[SerializeField]
		private WalletConnectSharp.Core.Models.ClientMeta _walletAppData;
		public WalletConnectSharp.Core.Models.ClientMeta WalletAppData => _walletAppData;


#if UNITY_EDITOR

		private void OnValidate()
		{
			OnValidateAdMediationPlatform();
		}

		private void OnValidateAdMediationPlatform()
		{
			if (_selectedAdMediationPlatform != _adMediationPlatform)
			{
				_selectedAdMediationPlatform = _adMediationPlatform;

				if (_selectedAdMediationPlatform != AdMediationPlatform.None)
				{
					AddMediationScriptingDefineSymbol(BuildTargetGroup.iOS);
					AddMediationScriptingDefineSymbol(BuildTargetGroup.Android);
				}
				else
				{
					ClearMediationScriptingDefineSymbols(BuildTargetGroup.iOS);
					ClearMediationScriptingDefineSymbols(BuildTargetGroup.Android);
				}
			}
		}

		private List<string> GetAllScriptingDefineSymbolsExceptMediation(BuildTargetGroup targetPlatform)
		{
			string allDefinesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetPlatform);
			return allDefinesString.Split(';').Except(GetMediationDefineSymbolsFromEnum()).ToList();
		}

		private void AddMediationScriptingDefineSymbol(BuildTargetGroup targetPlatform)
		{
			var allDefines = GetAllScriptingDefineSymbolsExceptMediation(targetPlatform);
			allDefines.Add(AD_MEDIATION_DEFINE_PREFIX + _selectedAdMediationPlatform.ToString().ToUpper());

			PlayerSettings.SetScriptingDefineSymbolsForGroup(targetPlatform, string.Join(";", allDefines.ToArray()));
		}

		private void ClearMediationScriptingDefineSymbols(BuildTargetGroup targetPlatform)
		{
			PlayerSettings.SetScriptingDefineSymbolsForGroup(
				targetPlatform,
				string.Join(";", GetAllScriptingDefineSymbolsExceptMediation(targetPlatform).ToArray())
			);
		}

		private List<string> GetMediationDefineSymbolsFromEnum()
		{
			var enumList = Enum.GetValues(typeof(AdMediationPlatform)).Cast<AdMediationPlatform>().ToList();
			List<string> symbols = new();

			foreach (var symbol in enumList)
			{
				symbols.Add(AD_MEDIATION_DEFINE_PREFIX + symbol.ToString().ToUpper());
			}

			return symbols;
		}

		[MenuItem("iDosGames/Settings")]
		private static void OpenSettings()
		{
			Selection.activeObject = Instance;
		}

#endif

	}
}