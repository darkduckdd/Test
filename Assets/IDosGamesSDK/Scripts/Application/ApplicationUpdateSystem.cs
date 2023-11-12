using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

namespace IDosGames
{
	public class ApplicationUpdateSystem : MonoBehaviour
	{
		[SerializeField] private PopUpApplicationUpdate _popUp;

		private void OnEnable()
		{
			PlayFabDataService.AllDataUpdated += CheckForUpdates;
		}

		private void OnDisable()
		{
			PlayFabDataService.AllDataUpdated -= CheckForUpdates;
		}

		private void CheckForUpdates()
		{
			var UpdateData = PlayFabDataService.GetTitleData(TitleDataKey.application_update);

			if (UpdateData == string.Empty)
			{
				return;
			}

			var jsonUpdateData = JsonConvert.DeserializeObject<JObject>(UpdateData);

			JObject deviceUpdateData = null;

#if UNITY_IOS
			deviceUpdateData = (JObject)jsonUpdateData[JsonProperty.IOS];
#elif UNITY_ANDROID
			deviceUpdateData = (JObject)jsonUpdateData[JsonProperty.ANDROID];
#endif
			if (deviceUpdateData == null)
			{
				return;
			}

			var version = deviceUpdateData[JsonProperty.VERSION];

			if ($"{version}" == $"{Application.version}")
			{
				return;
			}

			var urgencyData = deviceUpdateData[JsonProperty.URGENCY];
			Enum.TryParse($"{urgencyData}", out UpdateUrgency urgency);

			if (urgency == UpdateUrgency.NoUpdates)
			{
				return;
			}

			var linkToUpdate = deviceUpdateData[JsonProperty.LINK];

			_popUp.Set(urgency, $"{version}", $"{linkToUpdate}");
			_popUp.gameObject.SetActive(true);
		}
	}
}