using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace IDosGames
{
	public class LeaderboardView : MonoBehaviour
	{
		[SerializeField] private TMP_Text _title;
		[SerializeField] private TMP_Text _valueName;
		[SerializeField] private Transform _rowsParent;
		[SerializeField] private LeaderboardRow _rowPrefab;
		[SerializeField] private Sprite[] _rankIcons;
		[SerializeField] private ButtonRefreshLeaderboard _refreshButton;
		[SerializeField] private LeaderboardTimer _timer;

		public void SetTitle(string title)
		{
			_title.text = title;
		}

		public void SetStatValueName(string name)
		{
			_valueName.text = name;
		}

		public void SetTimer(string frequencyData)
		{
			if (frequencyData == null)
			{
				return;
			}

			bool isCorrectFrequency = Enum.TryParse(frequencyData.ToLower(), out LeaderboardUpdateFrequency frequency);

			_timer.gameObject.SetActive(isCorrectFrequency);

			if (!isCorrectFrequency)
			{
				return;
			}

			_timer.UpdateTimer(frequency);
		}

		public void SetRows(List<PlayerLeaderboardEntry> leaderboard)
		{
			foreach (Transform child in _rowsParent)
			{
				Destroy(child.gameObject);
			}

			CreateCurrentPlayerRow(leaderboard);
			CreateOtherPlayerRows(leaderboard);
			_refreshButton.SetInteractable(false);
		}

		private void CreateCurrentPlayerRow(List<PlayerLeaderboardEntry> leaderboard)
		{
			bool isCurrentPlayerOnLeaderboard = false;
			PlayerLeaderboardEntry currentPlayer = new() { PlayFabId = PlayFabAuthService.PlayFabID, Position = -1 };

			foreach (var player in leaderboard)
			{
				if (player.PlayFabId == PlayFabAuthService.PlayFabID)
				{
					currentPlayer = player;
					isCurrentPlayerOnLeaderboard = true;
				}
			}

			var statValue = GetStatValueForCurrentPlayer(isCurrentPlayerOnLeaderboard, currentPlayer, leaderboard);

			CreateRow(currentPlayer.Position, currentPlayer.PlayFabId, statValue);
		}

		private string GetStatValueForCurrentPlayer(bool isCurrentPlayerOnLeaderboard, PlayerLeaderboardEntry currentPlayer, List<PlayerLeaderboardEntry> leaderboard)
		{
			string statValue;
			if (isCurrentPlayerOnLeaderboard)
			{
				statValue = $"{currentPlayer.StatValue}";
			}
			else
			{
				var lastPLayerOnLeaderboardStatValue = leaderboard.Count > 0 ? leaderboard.Last().StatValue : 0;
				statValue = lastPLayerOnLeaderboardStatValue > 0 ? $"<{lastPLayerOnLeaderboardStatValue}" : $"{0}";
			}

			return statValue;
		}

		private void CreateOtherPlayerRows(List<PlayerLeaderboardEntry> leaderboard)
		{
			foreach (var player in leaderboard)
			{
				if (player.PlayFabId == PlayFabAuthService.PlayFabID)
				{
					continue;
				}

				CreateRow(player.Position, player.PlayFabId, $"{player.StatValue}");
			}
		}

		private void CreateRow(int position, string playFabID, string statValue)
		{
			var row = Instantiate(_rowPrefab, _rowsParent);
			row.Set(position, playFabID, statValue, GetRankIcon(position));
		}

		private Sprite GetRankIcon(int position)
		{
			if (_rankIcons.Length > position && position >= 0)
			{
				return _rankIcons[position];
			}

			return null;
		}
	}
}