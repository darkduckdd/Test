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
	public class PlayFabDataService
	{
		public const string CATALOG_SKIN = "Skin";

		public static event Action AllDataRequested;
		public static event Action<PlayFabError> AllDataRequestError;
		public static event Action AllDataUpdated;

		private static readonly Dictionary<TitleDataKey, string> _titleData = new();
		private static readonly Dictionary<PlayerDataKey, string> _playerData = new();

		private static readonly Dictionary<string, RarityType> _skinCollectionRarity = new();
		private static readonly Dictionary<string, float> _skinCollectionProfit = new();
		private static readonly Dictionary<string, SkinCatalogItem> _skinItems = new();

		private static PlayFabDataService _instance;

		public static PlayFabDataService Instance => _instance;

		public PlayFabDataService()
		{
			_instance = this;
		}

		[RuntimeInitializeOnLoadMethod]
		private static void Initialize()
		{
			_instance = new();
		}

		public static void RequestAllData() // Inventory -> TitleData -> UserReadOnlyData -> SkinCatalogItems -> PlayerProfile
		{
			RequestUserInventory();

			AllDataRequested?.Invoke();
		}

		private static void OnAllDataRequestError(PlayFabError error)
		{
			if (error.ErrorMessage == null)
			{
				return;
			}

			AllDataRequestError?.Invoke(error);
		}

		private static void RequestUserInventory()
		{
			PlayFabService.RequestUserInventory(
				resultCallback: OnUserInventoryReceived,
				notConnectionErrorCallback: OnRequestUserInventoryError,
				connectionErrorCallback: () => RequestAllData()
			);
		}

		private static void OnUserInventoryReceived(GetUserInventoryResult result)
		{
			RequestTitleData();
		}

		private static void OnRequestUserInventoryError(PlayFabError error)
		{
			OnAllDataRequestError(error);
			RequestTitleData();
		}

		private static void RequestTitleData()
		{
			PlayFabService.RequestTitleData(
				resultCallback: OnTitleDataReceived,
				notConnectionErrorCallback: OnRequestTitleDataError,
				connectionErrorCallback: () => RequestAllData()
			);
		}

		private static void OnTitleDataReceived(GetTitleDataResult result)
		{
			UpdateTitleData(result);

			RequestUserReadOnlyData();
		}

		private static void OnRequestTitleDataError(PlayFabError error)
		{
			OnAllDataRequestError(error);
			RequestUserReadOnlyData();
		}

		private static void RequestUserReadOnlyData()
		{
			PlayFabService.RequestUserReadOnlyData(
				resultCallback: OnUserReadOnlyDataReceived,
				notConnectionErrorCallback: OnRequestUserReadOnlyDataError,
				connectionErrorCallback: () => RequestAllData()
			);
		}

		private static void OnUserReadOnlyDataReceived(GetUserDataResult result)
		{
			UpdateUserReadOnlyData(result);
			RequestSkinCatalogItems();
		}

		private static void OnRequestUserReadOnlyDataError(PlayFabError error)
		{
			OnAllDataRequestError(error);
			RequestSkinCatalogItems();
		}

		private static void RequestSkinCatalogItems()
		{
			PlayFabService.RequestCatalogItems(
				catalogName: CATALOG_SKIN,
				resultCallback: OnSkinCatalogItemsReceived,
				notConnectionErrorCallback: OnRequestSkinCatalogItemsError,
				connectionErrorCallback: () => RequestAllData()
			);
		}

		/// TODO: Add Player Profile in PlayFab and locally decomment

		private static void OnSkinCatalogItemsReceived(GetCatalogItemsResult result)
		{
			UpdateSkinItems(result);
			///RequestPlayerProfile();
		}

		private static void OnRequestSkinCatalogItemsError(PlayFabError error)
		{
			OnAllDataRequestError(error);
			///RequestPlayerProfile();
		}

		private static void RequestPlayerProfile()
		{
			PlayFabService.RequestPlayerProfile(
				resultCallback: OnPlayerProfileReceived,
				notConnectionErrorCallback: OnRequestPlayerProfileError,
				connectionErrorCallback: () => RequestAllData()
			);
		}

		private static void OnPlayerProfileReceived(GetPlayerProfileResult result)
		{
			UpdateVIPSubscriptionStatus(result);

			AllDataUpdated?.Invoke();
		}

		private static void OnRequestPlayerProfileError(PlayFabError error)
		{
			OnAllDataRequestError(error);
		}

		private static void UpdateTitleData(GetTitleDataResult result)
		{
			foreach (var data in result.Data)
			{
				if (Enum.TryParse(data.Key, true, out TitleDataKey dataKey))
				{
					_titleData[dataKey] = data.Value;
				}
			}
		}

		private static void UpdateUserReadOnlyData(GetUserDataResult result)
		{
			foreach (var data in result.Data)
			{
				if (Enum.TryParse(data.Key, true, out PlayerDataKey dataKey))
				{
					_playerData[dataKey] = data.Value.Value;
				}
			}
		}

		private static void UpdateVIPSubscriptionStatus(GetPlayerProfileResult result)
		{
			bool status = false;

			if (result.PlayerProfile.Memberships.Count() > 0)
			{
				for (int i = 0; i < result.PlayerProfile.Memberships[0].Subscriptions.Count(); i++)
				{
					if (result.PlayerProfile.Memberships[0].Subscriptions[i].IsActive)
					{
						status = true;
						break;
					}
				}
			}

			Debug.Log(status);
		}

		public static string GetTitleData(TitleDataKey dataKey)
		{
			_titleData.TryGetValue(dataKey, out string data);

			return $"{data}";
		}

		public static string GetPlayerData(PlayerDataKey dataKey)
		{
			_playerData.TryGetValue(dataKey, out string data);

			return $"{data}";
		}

		private static void UpdateSkinItems(GetCatalogItemsResult result)
		{
			if (result.Catalog == null)
			{
				return;
			}

			SetSkinCollectionRarityAndProfit();

			foreach (var item in result.Catalog)
			{
				var customData = JsonConvert.DeserializeObject<JObject>(item.CustomData);

				customData.TryGetValue(JsonProperty.IMAGE_PATH, out JToken imagePath).ToString();
				customData.TryGetValue(JsonProperty.COLLECTION, out JToken collection).ToString();

				var rarity = GetSkinRarityByCollection($"{collection}");
				var profit = GetSkinProfitByCollection($"{collection}");

				_skinItems[item.ItemId] = new(item, $"{collection}", $"{imagePath}", profit, rarity);
			}

			AllDataUpdated?.Invoke();
		}

		private static void SetSkinCollectionRarityAndProfit()
		{
			string collectionRarityData = GetTitleData(TitleDataKey.skin_collection_rarity);

			var collectionRarities = JsonConvert.DeserializeObject<JArray>(collectionRarityData);

			foreach (var collectionRarity in collectionRarities)
			{
				var collections = JsonConvert.DeserializeObject<List<string>>($"{collectionRarity[JsonProperty.COLLECTIONS]}");
				Enum.TryParse($"{collectionRarity[JsonProperty.RARITY]}", true, out RarityType rarity);
				float.TryParse($"{collectionRarity[JsonProperty.PROFIT]}", out float profit);

				foreach (var collection in collections)
				{
					_skinCollectionRarity[collection] = rarity;
					_skinCollectionProfit[collection] = profit;
				}
			}

		}

		public static SkinCatalogItem GetSkinItem(string itemID)
		{
			_skinItems.TryGetValue(itemID, out SkinCatalogItem item);

			return item;
		}

		private static RarityType GetSkinRarityByCollection(string collection)
		{
			_skinCollectionRarity.TryGetValue(collection, out RarityType rarity);

			return rarity;
		}

		private static float GetSkinProfitByCollection(string collection)
		{
			_skinCollectionProfit.TryGetValue(collection, out float profit);

			return profit;
		}
	}
}