using UnityEngine;

namespace IDosGames
{
	public class VirtualCurrencyBar : CurrencyBar
	{
		[SerializeField] private VirtualCurrencyID _virtualCurrencyID;

		private void OnEnable()
		{
			UpdateAmount();
			UserInventory.InventoryUpdated += UpdateAmount;
		}

		private void OnDisable()
		{
			UserInventory.InventoryUpdated -= UpdateAmount;
		}

		public override void UpdateAmount()
		{
			Amount = UserInventory.GetVirtualCurrencyAmount(_virtualCurrencyID);
		}
	}
}