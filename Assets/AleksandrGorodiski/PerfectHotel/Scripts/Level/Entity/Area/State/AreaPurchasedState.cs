using System.Collections.Generic;
using Injection;

namespace Game.Level.Area.AreaState
{
    public sealed class AreaPurchasedState : AreaState
    {
        [Inject] private GameManager _gameManager;

        private Dictionary<AreaController, HotelWallView> _wallAreaMap;

        public override void Initialize()
        {
            _wallAreaMap = new Dictionary<AreaController, HotelWallView>();
            foreach (var wall in _area.View.HidingWallsViews)
            {
                var area = _gameManager.FindArea(wall.HideOnArea);                    
                _wallAreaMap.Add(area, wall);

                if (area.Model.IsPurchased)
                    OnAreaPurchased(area);
            }

            _area.View.HudView.gameObject.SetActive(false);

            OnLvlChanged(_gameManager.Model.LoadLvl());

            _gameManager.LEVEL_CHANGED += OnLvlChanged;
            _gameManager.AREA_PURCHASED += OnAreaPurchased;
        }

        public override void Dispose()
        {
            _gameManager.LEVEL_CHANGED -= OnLvlChanged;
            _gameManager.AREA_PURCHASED -= OnAreaPurchased;
        }

        private void OnAreaPurchased(AreaController area)
        {
            if (_wallAreaMap.ContainsKey(area))
            {
                var wall = _wallAreaMap[area];
                wall.gameObject.SetActive(false);
            }
        }

        private void OnLvlChanged(int lvl)
        {
            _area.View.UpdateFloors(lvl);
            _area.View.UpdateHidingWalls(lvl);
            _area.View.UpdatePermanentWalls(lvl);
        }
    }
}

