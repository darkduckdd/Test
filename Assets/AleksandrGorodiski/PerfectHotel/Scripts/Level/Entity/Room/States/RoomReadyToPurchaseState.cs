using Game.Level.Player.PlayerState;
using Game.UI;
using Injection;

namespace Game.Level.Room.RoomState
{
    public sealed class RoomReadyToPurchaseState : RoomState
    {
        [Inject] private GameManager _gameManager;
        [Inject] private GameView _gameView;

        public override void Initialize()
        {
            _room.View.HudView.gameObject.SetActive(true);
            _room.View.HudView.ReadyToPuchase();
            _room.View.MeshesHolder.SetActive(false);
            _room.View.HideWallsPurchasedState();

            OnLvlChanged(_gameManager.Model.LoadLvl());

            _room.ItemBuyUpdate.PLAYER_ON_ITEM += PlayerOnItem;

            _gameManager.AddItem(_room.ItemBuyUpdate);

            _gameManager.LEVEL_CHANGED += OnLvlChanged;
        }

        public override void Dispose()
        {
            _gameManager.LEVEL_CHANGED -= OnLvlChanged;

            _room.ItemBuyUpdate.PLAYER_ON_ITEM -= PlayerOnItem;

            _gameManager.RemoveItem(_room.ItemBuyUpdate);
        }

        private void OnLvlChanged(int lvl)
        {
            _room.View.UpdateWallsHiddenState(lvl);
        }

        private void PlayerOnItem(ItemController item)
        {
            if (!SideAdvertising.IsActive)
            {
                if (_gameManager.Model.Cash <= 0)
                {
                    SideAdvertising.ShowPanel?.Invoke(_room.Model.PricePurchase, IDosGames.VirtualCurrencyID.MO);
                    return;
                }
                if (_gameManager.Model.Cash < _room.Model.PricePurchase)
                {
                    SideAdvertising.ShowPanel?.Invoke(_room.Model.PricePurchase, IDosGames.VirtualCurrencyID.MO);
                    return;
                }
            }

            if (_gameManager.Model.Cash <= 0) return;

            int amount = _room.Model.TransferRate;

            if (_room.Model.PriceNextLvl % amount != 0)
            {
                amount = _room.Model.PriceNextLvl % amount;
            }
            if (_gameManager.Model.Cash < amount)
            {
                amount = _gameManager.Model.Cash;
            }
            _gameManager.Model.Cash -= amount;
            _gameManager.Model.SetChanged();
            _gameManager.Model.Save();

            _room.Model.PricePurchase -= amount;
            _room.Model.SetChanged();

            _gameManager.FireFlyToRemoveCash(_room.View.HudView.transform.position);

            if (_room.Model.PricePurchase > 0) return;

            _gameManager.Model.SavePlaceIsPurchased(_room.Model.ID);
            _room.Model.IsPurchased = _gameManager.Model.LoadPlaceIsPurchased(_room.Model.ID);
            _room.Model.SetChanged();

            _gameView.CameraController.SetTarget(_room.View.transform);
            _gameView.CameraController.ZoomIn(true);

            _gameManager.FireAddGameProgress(_gameManager.Player.View.transform.position, _room.Model.PurchaseProgressReward);

            _room.SwitchToState(new RoomAvailableState());
            _gameManager.Player.SwitchToState(new PlayerPauseState());
        }
    }
}