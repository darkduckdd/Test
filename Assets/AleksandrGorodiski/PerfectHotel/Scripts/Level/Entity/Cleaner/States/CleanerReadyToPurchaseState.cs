using Game.Level.Player.PlayerState;

namespace Game.Level.Cleaner.CleanerState
{
    public sealed class CleanerReadyToPurchaseState : CleanerState
    {
        public override void Initialize()
        {
            _cleaner.View.HudView.gameObject.SetActive(true);
            _cleaner.View.HudView.ReadyToPuchase();

            _cleaner.View.UnitView.Hide();

            _cleaner.ItemBuyUpdate.PLAYER_ON_ITEM += PlayerOnItem;

            _gameManager.AddItem(_cleaner.ItemBuyUpdate);
        }

        public override void Dispose()
        {
            _cleaner.ItemBuyUpdate.PLAYER_ON_ITEM -= PlayerOnItem;

            _gameManager.RemoveItem(_cleaner.ItemBuyUpdate);
        }

        private void PlayerOnItem(ItemController item)
        {

            if (!SideAdvertising.IsActive)
            {
                if (_gameManager.Model.Cash <= 0)
                {
                    SideAdvertising.ShowPanel?.Invoke(_cleaner.Model.PricePurchase, IDosGames.VirtualCurrencyID.MO);

                    return;
                }
                if (_gameManager.Model.Cash < _cleaner.Model.PricePurchase)
                {
                    SideAdvertising.ShowPanel?.Invoke(_cleaner.Model.PricePurchase, IDosGames.VirtualCurrencyID.MO);
                    return;
                }
            }
            if (_gameManager.Model.Cash <= 0) return;

            int amount = _cleaner.Model.TransferRate;
            if (_cleaner.Model.PricePurchase % amount != 0)
            {
                amount = _cleaner.Model.PricePurchase % amount;
            }

            if (_gameManager.Model.Cash < amount)
            {
                amount = _gameManager.Model.Cash;
            }
            _gameManager.Model.Cash -= amount;
            _gameManager.Model.SetChanged();
            _gameManager.Model.Save();

            _cleaner.Model.PricePurchase -= amount;
            _cleaner.Model.SetChanged();

            _gameManager.FireFlyToRemoveCash(_cleaner.View.HudView.transform.position);

            if (_cleaner.Model.PricePurchase > 0) return;

            _gameManager.Model.SavePlaceIsPurchased(_cleaner.Model.ID);
            _cleaner.Model.IsPurchased = _gameManager.Model.LoadPlaceIsPurchased(_cleaner.Model.ID);
            _cleaner.Model.SetChanged();

            _cleaner.UnitView.PlayUnitParticles();

            _gameView.CameraController.SetTarget(_cleaner.UnitView.transform);
            _gameView.CameraController.ZoomIn(true);

            _cleaner.SwitchToState(new CleanerIdleState());
            _gameManager.Player.SwitchToState(new PlayerPauseState());
        }
    }
}