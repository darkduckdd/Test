using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlayFab.ClientModels;
using System.Collections;
using UnityEngine;

namespace IDosGames
{
	public class ChestRewardRoom : MonoBehaviour
	{
		private const float CHANGE_STATE_DELAY = 1.0f;
		[SerializeField] private Chest _chest;
		[SerializeField] private ChestRewardCard _rewardCard;
		[SerializeField] private ButtonContinue _buttonContinue;
		[SerializeField] private MainSceneViewObjects _mainSceneViewObjects;

		private ChestRoomWaitState _currentState;

		private void OnEnable()
		{
			SetActiveMainSceneObjects(false);
			SetInitialState();
		}

		private void OnDisable()
		{
			SetActiveMainSceneObjects(true);
		}

		private void Start()
		{
			_buttonContinue.ResetListener(OnClickContinue);
		}

		public void ShowReward(ChestRarityType rarity, SkinCatalogItem item)
		{
			SetChest(rarity);
			SetReward(item);

			SetActiveRoom(true);
		}

		private void SetChest(ChestRarityType rarity)
		{
			_chest.SetMaterialByRarity(rarity);
		}

		private void SetReward(SkinCatalogItem item)
		{
			var icon = Resources.Load<Sprite>(item.ImagePath);

			_rewardCard.Set(item.Rarity, icon, item.DisplayName);
		}

		private void SetInitialState()
		{
			_currentState = ChestRoomWaitState.DropChest;
			_rewardCard.Animation.ResetAnimation();
			_chest.Animation.Disappear();
		}

		private void OnClickContinue()
		{
			switch (_currentState)
			{
				case ChestRoomWaitState.DropChest:
					DropChest();
					break;
				case ChestRoomWaitState.OpenChest:
					OpenChest();
					break;
				case ChestRoomWaitState.LeaveRoom:
					LeaveRoom();
					break;
			}
		}

		private void DropChest()
		{
			_chest.Animation.Drop();
			StartCoroutine(ChangeToNextState());
		}

		private void OpenChest()
		{
			_chest.Animation.Open();
			_rewardCard.Animation.StartAnimation();
			StartCoroutine(ChangeToNextState());
		}

		private void LeaveRoom()
		{
			SetActiveRoom(false);
		}

		private IEnumerator ChangeToNextState()
		{
			SetActiveContinueButton(false);

			yield return new WaitForSecondsRealtime(CHANGE_STATE_DELAY);

			_currentState++;

			SetActiveContinueButton(true);
		}

		private void SetActiveRoom(bool active)
		{
			gameObject.SetActive(active);
		}

		private void SetActiveContinueButton(bool active)
		{
			_buttonContinue.SetActive(active);
		}

		private void SetActiveMainSceneObjects(bool active)
		{
			if (_mainSceneViewObjects == null)
			{
				return;
			}

			_mainSceneViewObjects.gameObject.SetActive(active);
		}
	}
}