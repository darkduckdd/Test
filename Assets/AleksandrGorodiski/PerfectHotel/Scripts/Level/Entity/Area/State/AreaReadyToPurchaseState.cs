using Injection;

namespace Game.Level.Area.AreaState
{
    public sealed class AreaReadyToPurchaseState : AreaState
    {
        [Inject] private GameManager _gameManager;

        public override void Initialize()
        {
            _area.Model.IsLocked = false;
            _area.Model.SetChanged();

            _area.View.HudView.gameObject.SetActive(true);
            _area.View.HudView.ReadyToPuchase();

            _area.View.HideFloors();
            _area.View.HideHidingWalls();
            _area.View.HidePermanentWalls();

            _area.ItemBuyUpdate.PLAYER_ON_ITEM += PlayerOnItem;

            _gameManager.AddItem(_area.ItemBuyUpdate);
        }

        public override void Dispose()
        {
            _area.ItemBuyUpdate.PLAYER_ON_ITEM -= PlayerOnItem;

            _gameManager.RemoveItem(_area.ItemBuyUpdate);
        }

        private void PlayerOnItem(ItemController item)
        {
            if (!SideAdvertising.IsActive)
            {
                if (_gameManager.Model.Cash <= 0)
                {
                    SideAdvertising.ShowPanel?.Invoke(_area.Model.PricePurchase, IDosGames.VirtualCurrencyID.MO);

                    return;
                }
                if (_gameManager.Model.Cash < _area.Model.PricePurchase)
                {
                    SideAdvertising.ShowPanel?.Invoke(_area.Model.PricePurchase, IDosGames.VirtualCurrencyID.MO);

                    return;
                }
            }

            if (_gameManager.Model.Cash <= 0)
            {
                return;
            }

            int amount = _area.Model.TransferRate;
            if (_area.Model.PricePurchase % amount != 0)
            {
                amount = _area.Model.PricePurchase % amount;
            }
            if (_gameManager.Model.Cash < amount)
            {
                amount = _gameManager.Model.Cash;
            }

            _gameManager.Model.Cash -= amount;
            _gameManager.Model.SetChanged();
            _gameManager.Model.Save();

            _area.Model.PricePurchase -= amount;
            _area.Model.SetChanged();

            _gameManager.FireFlyToRemoveCash(_area.View.HudView.transform.position);

            if (_area.Model.PricePurchase > 0) return;

            _gameManager.Model.SavePlaceIsPurchased(_area.Model.ID);
            _area.Model.IsPurchased = _gameManager.Model.LoadPlaceIsPurchased(_area.Model.ID);
            _area.Model.SetChanged();

            _gameManager.FireAreaPurchased(_area);

            _area.SwitchToState(new AreaPurchasedState());
        }
    }
}

