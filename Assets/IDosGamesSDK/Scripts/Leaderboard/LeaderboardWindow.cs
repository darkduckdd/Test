using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace IDosGames
{
	public class LeaderboardWindow : MonoBehaviour
	{
		public const int MAX_DISPLAY_PLACES_COUNT = 100;

		[SerializeField] private LeaderboardView _view;
		[SerializeField] private LeaderboardDescription _description;

		private void Start()
		{
			Refresh();
		}

		public void Refresh()
		{
			var titleData = PlayFabDataService.GetTitleData(TitleDataKey.leaderboard);
			var leaderboardData = JsonConvert.DeserializeObject<JObject>(titleData);

			if (leaderboardData == null)
			{
				return;
			}

			_view.SetTitle($"{leaderboardData[JsonProperty.NAME]}");
			_view.SetStatValueName($"{leaderboardData[JsonProperty.VALUE_NAME]}");
			_view.SetTimer($"{leaderboardData[JsonProperty.FREQUENCY]}");

			var leaderboardID = $"{leaderboardData[JsonProperty.ID]}";
			RequestLeaderboard(leaderboardID);

			_description.Initialize(leaderboardData);
		}

		private void RequestLeaderboard(string leaderboardID)
		{
			Loading.ShowTransparentPanel();
			PlayFabService.RequestLeaderboard(leaderboardID, MAX_DISPLAY_PLACES_COUNT, OnSuccessGetLeaderboard, OnErrorGetLeaderboard, () => RequestLeaderboard(leaderboardID));
		}

		private void OnSuccessGetLeaderboard(GetLeaderboardResult result)
		{
			Loading.HideAllPanels();

			if (result == null)
			{
				return;
			}

			if (result.Leaderboard == null)
			{
				return;
			}

			_view.SetRows(result.Leaderboard);
		}

		private void OnErrorGetLeaderboard(PlayFabError error)
		{
			Message.Show("Something went wrong. Ñontact technical support.");
		}
	}
}