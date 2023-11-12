using UnityEngine;

namespace IDosGames
{
	public class PlayFabInitialDataLoader : MonoBehaviour
	{
		private void Start()
		{
			LoadData();
		}

		private void LoadData()
		{
			PlayFabDataService.RequestAllData();
		}
	}
}