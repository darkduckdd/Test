using Newtonsoft.Json;
using PlayFab.ClientModels;
using UnityEngine;

namespace IDosGames
{
	public class ChestWindow : MonoBehaviour
	{
		[SerializeField] private ChestRewardRoom _rewardRoom;

		private ChestRarityType _chestRarity;

		public void TryToOpenChest(ChestRarityType rarity)
		{
			string functionName = "";

			switch (rarity)
			{
				case ChestRarityType.Common:
					functionName = PlayFabCloudFunctionName.CHEST_GET_COMMON_REWARD;
					break;
				case ChestRarityType.Rare:
					functionName = PlayFabCloudFunctionName.CHEST_GET_RARE_REWARD;
					break;
				case ChestRarityType.Legendary:
					functionName = PlayFabCloudFunctionName.CHEST_GET_LEGENDARY_REWARD;
					break;
			}

			_chestRarity = rarity;
			PlayFabService.ExecuteCloudFunction(functionName, OnSuccessResponseChestResult);
		}

		private void OnSuccessResponseChestResult(ExecuteCloudScriptResult result)
		{
			if (result != null && result.FunctionResult != null)
			{
				var itemID = result.FunctionResult.ToString();

				var item = PlayFabDataService.GetSkinItem(itemID);

				_rewardRoom.ShowReward(_chestRarity, item);

				PlayFabService.RequestUserInventory();
			}
			else
			{
				Message.Show("Something went wrong. Ñontact technical support.");
			}
		}
	}
}