using UnityEngine;
using UnityEngine.UI;

namespace IDosGames
{
	public class SpinButton : MonoBehaviour
	{
		[SerializeField] private SpinTicketType _spinType;
		[SerializeField] private Button _button;
		public Button Button => _button;

		private void OnEnable()
		{
			UpdateUI();
			UserInventory.InventoryUpdated += UpdateUI;
		}

		private void OnDisable()
		{
			UserInventory.InventoryUpdated -= UpdateUI;
		}

		protected virtual void UpdateUI()
		{
			int amount = UserInventory.GetSpinTicketAmount(_spinType);

			_button.interactable = amount > 0;
		}
	}
}