using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IDosGames
{
	public class PlayFabService
	{
		private static PlayFabService _instance;

		public static PlayFabService Instance => _instance;

		public PlayFabService()
		{
			_instance = this;
		}

		[RuntimeInitializeOnLoadMethod]
		private static void Initialize()
		{
			_instance = new();
		}

		public static event Action PlayFabError;
		public static event Action<Action> ConnectionError;

		public static event Action<GetUserInventoryResult> UserInventoryReceived;

		public static event Action CloudFunctionCalled;
		public static event Action CloudFunctionResponsed;

		public static void RequestUserInventory(Action<GetUserInventoryResult> resultCallback = null, Action<PlayFabError> notConnectionErrorCallback = null, Action connectionErrorCallback = null)
		{
			PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
				result =>
				{
					if (result != null)
					{
						UserInventoryReceived?.Invoke(result);
						resultCallback?.Invoke(result);
					}
				},
				error => OnPlayFabError(error, notConnectionErrorCallback, connectionErrorCallback));
		}

		public static void RequestTitleData(Action<GetTitleDataResult> resultCallback = null, Action<PlayFabError> notConnectionErrorCallback = null, Action connectionErrorCallback = null)
		{
			PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(),
				result =>
				{
					if (result != null)
					{
						resultCallback?.Invoke(result);
					}
				},
				error => OnPlayFabError(error, notConnectionErrorCallback, connectionErrorCallback));
		}

		public static void RequestLeaderboard(string leaderboardID, int maxResultCount, Action<GetLeaderboardResult> resultCallback = null, Action<PlayFabError> notConnectionErrorCallback = null, Action connectionErrorCallback = null)
		{
			PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest
			{
				StatisticName = leaderboardID,
				MaxResultsCount = maxResultCount
			},
			result =>
			{
				if (result != null)
				{
					resultCallback?.Invoke(result);
				}
			},
			error => OnPlayFabError(error, notConnectionErrorCallback, connectionErrorCallback));
		}

		public static void RequestUserReadOnlyData(Action<GetUserDataResult> resultCallback = null, Action<PlayFabError> notConnectionErrorCallback = null, Action connectionErrorCallback = null)
		{
			PlayFabClientAPI.GetUserReadOnlyData(new GetUserDataRequest(),
				result =>
				{
					if (result != null)
					{
						resultCallback?.Invoke(result);
					}
				},
				error => OnPlayFabError(error, notConnectionErrorCallback, connectionErrorCallback));
		}

		public static void RequestPlayerProfile(Action<GetPlayerProfileResult> resultCallback = null, Action<PlayFabError> notConnectionErrorCallback = null, Action connectionErrorCallback = null)
		{
			PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest()
			{
				ProfileConstraints = new PlayerProfileViewConstraints()
				{
					ShowLocations = true
				}
			},
			(result) =>
			{
				if (result != null)
				{
					resultCallback?.Invoke(result);
				}
			},
			error => OnPlayFabError(error, notConnectionErrorCallback, connectionErrorCallback));
		}

		public static void RequestCatalogItems(string catalogName, Action<GetCatalogItemsResult> resultCallback = null, Action<PlayFabError> notConnectionErrorCallback = null, Action connectionErrorCallback = null)
		{
			PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest()
			{
				CatalogVersion = catalogName
			},
			result =>
			{
				if (result != null)
				{
					resultCallback?.Invoke(result);
				}
			},
			error => OnPlayFabError(error, notConnectionErrorCallback, connectionErrorCallback));
		}

		public static void ExecuteCloudFunction(string functionName, Action<ExecuteCloudScriptResult> resultCallback = null, Action<PlayFabError> errorCallback = null, Dictionary<string, object> functionParameter = null)
		{
			CloudFunctionCalled?.Invoke();

			PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
			{
				FunctionName = functionName,
				FunctionParameter = functionParameter,
				GeneratePlayStreamEvent = true
			},
			result =>
			{
				CloudFunctionResponsed?.Invoke();
				resultCallback?.Invoke(result);
			},
			error => OnPlayFabError(error, errorCallback));
		}

		public static void OnPlayFabError(PlayFabError error, Action<PlayFabError> notConnectionErrorCallback, Action retryCallback = null)
		{
			PlayFabError?.Invoke();

			if (error == null)
			{
				return;
			}

			bool isConnectionError = Instance.CheckForConnectionError(error.Error);

			if (isConnectionError)
			{
				ConnectionError?.Invoke(retryCallback);
			}
			else
			{
				notConnectionErrorCallback?.Invoke(error);
			}
		}

		private bool CheckForConnectionError(PlayFabErrorCode errorCode)
		{
			return errorCode == PlayFabErrorCode.ConnectionError || errorCode == PlayFabErrorCode.ServiceUnavailable;
		}

	}
}