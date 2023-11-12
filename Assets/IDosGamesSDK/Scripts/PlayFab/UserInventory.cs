using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IDosGames
{
	public class UserInventory
	{
		private static UserInventory _instance;

		public static UserInventory Instance => _instance;

		public UserInventory()
		{
			_instance = this;

			PlayFabService.UserInventoryReceived += OnUserInventoryReceived;
			PlayFabIAPValidator.VIPSubscriptionValidated += OnVIPSubscriptionValidated;
		}

		~UserInventory()
		{
			PlayFabService.UserInventoryReceived -= OnUserInventoryReceived;
			PlayFabIAPValidator.VIPSubscriptionValidated -= OnVIPSubscriptionValidated;
		}

		private static readonly Dictionary<string, int> _eachItemAmounts = new();
		private static readonly Dictionary<VirtualCurrencyID, int> _virtualCurrencyAmounts = new();
		private static readonly Dictionary<VirtualCurrencyID, DateTime> _virtualCurrencytRechargeTimes = new();
		private static readonly Dictionary<SpinTicketType, int> _spinTickets = new();
		private static readonly Dictionary<ChestKeyFragmentType, int> _chestKeyFragments = new();

		public static event Action InventoryUpdated;

		public static bool HasVIPStatus { get; private set; }

		[RuntimeInitializeOnLoadMethod]
		private static void Initialize()
		{
			_instance = new();
		}

		private void OnVIPSubscriptionValidated()
		{

		}

		private static void OnUserInventoryReceived(GetUserInventoryResult result)
		{
			Instance.ResetAllData();

			Instance.SetEachItemAmounts(result.Inventory);
			Instance.SetVirtualCurrencyAmounts(result.VirtualCurrency);
			Instance.SetSpinTickets();
			Instance.SetChestKeyFragments();

			InventoryUpdated?.Invoke();
		}

		private void SetEachItemAmounts(List<ItemInstance> inventoryItems)
		{
			foreach (var item in inventoryItems)
			{
				int amount = _eachItemAmounts.ContainsKey(item.ItemId) ? _eachItemAmounts[item.ItemId] : 0;
				var remainingUses = item.RemainingUses;
				amount += remainingUses != null ? (int)remainingUses : 1;

				_eachItemAmounts[item.ItemId] = amount;
			}
		}

		private void SetVirtualCurrencyAmounts(Dictionary<string, int> virtualCurrencies)
		{
			foreach (var virtualCurency in virtualCurrencies)
			{
				if (Enum.TryParse(virtualCurency.Key, true, out VirtualCurrencyID virtualCurrencyID))
				{
					_virtualCurrencyAmounts[virtualCurrencyID] = virtualCurency.Value;
				}
			}
		}

		private void SetSpinTickets()
		{
			_spinTickets[SpinTicketType.Standard] = _eachItemAmounts.FirstOrDefault(x => x.Key == PlayFabItemID.STANDARD_SPIN_TICKET).Value;
			_spinTickets[SpinTicketType.Premium] = _eachItemAmounts.FirstOrDefault(x => x.Key == PlayFabItemID.PREMIUM_SPIN_TICKET).Value;
		}

		private void SetChestKeyFragments()
		{
			_chestKeyFragments[ChestKeyFragmentType.Common_1] = _eachItemAmounts.FirstOrDefault(x => x.Key == PlayFabItemID.COMMON_CHEST_KEY_FRAGMENT_1).Value;
			_chestKeyFragments[ChestKeyFragmentType.Common_2] = _eachItemAmounts.FirstOrDefault(x => x.Key == PlayFabItemID.COMMON_CHEST_KEY_FRAGMENT_2).Value;
			_chestKeyFragments[ChestKeyFragmentType.Common_3] = _eachItemAmounts.FirstOrDefault(x => x.Key == PlayFabItemID.COMMON_CHEST_KEY_FRAGMENT_3).Value;
			_chestKeyFragments[ChestKeyFragmentType.Rare_1] = _eachItemAmounts.FirstOrDefault(x => x.Key == PlayFabItemID.RARE_CHEST_KEY_FRAGMENT_1).Value;
			_chestKeyFragments[ChestKeyFragmentType.Rare_2] = _eachItemAmounts.FirstOrDefault(x => x.Key == PlayFabItemID.RARE_CHEST_KEY_FRAGMENT_2).Value;
			_chestKeyFragments[ChestKeyFragmentType.Rare_3] = _eachItemAmounts.FirstOrDefault(x => x.Key == PlayFabItemID.RARE_CHEST_KEY_FRAGMENT_3).Value;
			_chestKeyFragments[ChestKeyFragmentType.Legendary_1] = _eachItemAmounts.FirstOrDefault(x => x.Key == PlayFabItemID.LEGENDARY_CHEST_KEY_FRAGMENT_1).Value;
			_chestKeyFragments[ChestKeyFragmentType.Legendary_2] = _eachItemAmounts.FirstOrDefault(x => x.Key == PlayFabItemID.LEGENDARY_CHEST_KEY_FRAGMENT_2).Value;
			_chestKeyFragments[ChestKeyFragmentType.Legendary_3] = _eachItemAmounts.FirstOrDefault(x => x.Key == PlayFabItemID.LEGENDARY_CHEST_KEY_FRAGMENT_3).Value;
		}

		public static int GetVirtualCurrencyAmount(VirtualCurrencyID virtualCurrencyID)
		{
			_virtualCurrencyAmounts.TryGetValue(virtualCurrencyID, out int amount);

			return amount;
		}

		public static DateTime GetVirtualCurrencyRechargeTime(VirtualCurrencyID virtualCurrencyID)
		{
			_virtualCurrencytRechargeTimes.TryGetValue(virtualCurrencyID, out DateTime dateTime);

			return dateTime;
		}

		public static int GetSpinTicketAmount(SpinTicketType ticketType)
		{
			_spinTickets.TryGetValue(ticketType, out int amount);

			return amount;
		}

		public static int GetChestKeyFragmentAmount(ChestKeyFragmentType fragmentType)
		{
			_chestKeyFragments.TryGetValue(fragmentType, out int amount);

			return amount;
		}

		private void ResetAllData()
		{
			_virtualCurrencyAmounts.Clear();
			_virtualCurrencytRechargeTimes.Clear();
			_spinTickets.Clear();
			_eachItemAmounts.Clear();
		}
	}
}