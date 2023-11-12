using System.Collections.Generic;
using System.Linq;
using Game.Config;
using Game.Core;
using Injection;
using UnityEngine;

namespace Game.Level.Utility.UtilityState
{
    public sealed class UtilityPurchasedState : UtilityState
    {
        private const float _delay = 0.5f;

        [Inject] private GameConfig _config;
        [Inject] private Timer _timer;

        private readonly Dictionary<ItemController, float> _itemsMap;

        public UtilityPurchasedState()
        {
            _itemsMap = new Dictionary<ItemController, float>();
        }

        public override void Initialize()
        {
            _utility.View.MeshesHolder.SetActive(true);
            _utility.View.HideWallsHiddenState();

            foreach (var item in _utility.Items)
            {
                _itemsMap.Add(item, _delay);
                AddItem(item);
            }

            OnLvlChanged(_gameManager.Model.LoadLvl());

            _gameManager.LEVEL_CHANGED += OnLvlChanged;
            _timer.TICK += OnTick;
        }

        public override void Dispose()
        {
            _gameManager.LEVEL_CHANGED -= OnLvlChanged;
            _timer.TICK -= OnTick;
        }


        private void OnLvlChanged(int lvl)
        {
            _utility.View.OutsideWalls.MeshesVisibilityLvl(lvl);
        }

        private void OnTick()
        {
            foreach (var inventory in _itemsMap.Keys.ToList())
            {
                float value = _itemsMap[inventory];

                if (value >= _delay) return;

                value += Time.deltaTime;
                _itemsMap[inventory] = value;

                if (value >= _delay)
                    AddItem(inventory);
            }
        }

        void AddItem(ItemController item)
        {
            item.Model.Duration = _config.ToiletPaperFlyTime;
            item.Model.DurationNominal = item.Model.Duration;
            item.Model.SetChanged();

            _gameManager.AddItem(item);

            item.ITEM_FINISHED += OnItemFinished;
        }

        void OnItemFinished(ItemController item)
        {
            item.ITEM_FINISHED -= OnItemFinished;

            var type = item.Type;

            if (type == ItemType.UtilityToiletPaper)
                _gameManager.Model.Inventories.Add(InventoryType.ToiletPaper);

            _gameManager.Model.Save();
            _gameManager.Model.SetChanged();

            _itemsMap[item] = 0f;
        }
    }
}


