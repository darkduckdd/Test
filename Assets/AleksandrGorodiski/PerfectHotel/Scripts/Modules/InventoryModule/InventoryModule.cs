using System.Collections.Generic;
using System.Linq;
using Core;
using Game.Core;
using Game.Level.Inventory.States;
using Injection;
using UnityEngine;

public enum InventoryType
{
    None,
    ToiletPaper
}

namespace Game.Level.Inventory
{
    public sealed class InventoryModule : Module<InventoryModuleView>, IObserver
    {
        [Inject] private GameManager _gameManager;
        [Inject] private Context _context;
        [Inject] private Timer _timer;

        private readonly List<InventoryController> _inventories;
        private readonly Dictionary<int, InventoryController> _playerInventoriesMap;

        private int _inventoriesCount;

        public InventoryModule(InventoryModuleView view) : base(view)
        {
            _inventories = new List<InventoryController>();
            _playerInventoriesMap = new Dictionary<int, InventoryController>();
        }

        public override void Initialize()
        {
            CreateInventories();

            _gameManager.FLY_INVENTORY_ITEM += FlyInventoryItem;
            _timer.TICK += OnTick;

            _gameManager.Model.AddObserver(this);
        }

        public override void Dispose()
        {
            _gameManager.FLY_INVENTORY_ITEM -= FlyInventoryItem;
            _timer.TICK -= OnTick;

            _gameManager.Model.RemoveObserver(this);

            foreach (var inventory in _inventories)
            {
                inventory.REMOVE_INVENTORY -= RemoveInventory;
                inventory.Dispose();
            }
            _inventories.Clear();

            foreach (var pool in _view.InventoryMap.Values)
            {
                pool.ReleaseAllInstances();
            }
        }

        private void OnTick()
        {
            foreach (var index in _playerInventoriesMap.Keys.ToList())
            {
                var inventory = _playerInventoriesMap[index];
                inventory.View.transform.position = Vector3.Lerp(inventory.View.transform.position, _gameManager.Player.GetInventoryPosition(index), 0.8f);
            }
        }

        private InventoryController GetInventory(InventoryType type)
        {
            var view = _view.InventoryMap[type].Get<InventoryView>();
            var inventory = new InventoryController(view, type, _context);
            return inventory;
        }

        private void FlyInventoryItem(InventoryType type, Vector3 startPosition, Vector3 endPosition, float flyTime)
        {
            var inventory = GetInventory(type);
            inventory.View.transform.position = startPosition;
            inventory.SwitchToState(new InventoryFlyState(endPosition, flyTime));
            _inventories.Add(inventory);

            inventory.REMOVE_INVENTORY += RemoveInventory;
        }

        private void RemoveInventory(InventoryController inventory)
        {
            inventory.REMOVE_INVENTORY -= RemoveInventory;
            _view.InventoryMap[inventory.Type].Release(inventory.View);
            inventory.Dispose();
            _inventories.Remove(inventory);
        }

        public void OnObjectChanged(Observable observable)
        {
            if (_inventoriesCount == _gameManager.Model.Inventories.Count) return;

            foreach (var inventory in _playerInventoriesMap.Values.ToList())
            {
                inventory.FireRemoveInventory();
            }
            _playerInventoriesMap.Clear();

            CreateInventories();
        }

        private void CreateInventories()
        {
            _inventoriesCount = _gameManager.Model.Inventories.Count;
            for (int i = 0; i < _gameManager.Model.Inventories.Count; i++)
            {
                var type = _gameManager.Model.Inventories[i];
                var inventory = GetInventory(type);
                inventory.View.transform.position = _gameManager.Player.GetInventoryPosition(i);

                inventory.REMOVE_INVENTORY += RemoveInventory;
                _playerInventoriesMap.Add(i, inventory);
                _inventories.Add(inventory);
            }
        }
    }
}

