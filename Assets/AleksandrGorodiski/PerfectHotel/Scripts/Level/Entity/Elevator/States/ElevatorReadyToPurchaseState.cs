using Game.Level.Player.PlayerState;
using Injection;

namespace Game.Level.Elevator.ElevatorState
{
    public sealed class ElevatorReadyToPurchaseState : ElevatorState
    {
        [Inject] private GameManager _gameManager;

        public override void Initialize()
        {
            _elevator.Model.IsLocked = false;
            _elevator.Model.SetChanged();

            _elevator.View.HudView.gameObject.SetActive(true);
            _elevator.View.HudView.ReadyToPuchase();

            _elevator.View.MeshesHolder.SetActive(true);

            _elevator.View.HideWallsPurchasedState();
            _elevator.View.HideWallsHiddenState();

            _elevator.View.CloseDoor();

            OnLvlChanged(_gameManager.Model.LoadLvl());

            _elevator.BuyUpdateItem.PLAYER_ON_ITEM += PlayerOnItem;

            _gameManager.AddItem(_elevator.BuyUpdateItem);

            _gameManager.LEVEL_CHANGED += OnLvlChanged;
        }

        public override void Dispose()
        {
            _gameManager.LEVEL_CHANGED -= OnLvlChanged;

            _elevator.BuyUpdateItem.PLAYER_ON_ITEM -= PlayerOnItem;

            _gameManager.RemoveItem(_elevator.BuyUpdateItem);
        }

        private void OnLvlChanged(int lvl)
        {
            _elevator.View.OutsideWalls.MeshesVisibilityLvl(lvl);
        }

        private void PlayerOnItem(ItemController item)
        {

            if (!SideAdvertising.IsActive)
            {
                if (_gameManager.Model.Cash <= 0)
                {
                    SideAdvertising.ShowPanel?.Invoke(_elevator.Model.PricePurchase, IDosGames.VirtualCurrencyID.MO);

                    return;
                }
                if (_gameManager.Model.Cash < _elevator.Model.PricePurchase)
                {
                    SideAdvertising.ShowPanel?.Invoke(_elevator.Model.PricePurchase, IDosGames.VirtualCurrencyID.MO);
                    return;
                }
            }

            if (_gameManager.Model.Cash <= 0) return;

            int amount = _elevator.Model.TransferRate;
            if (_elevator.Model.PricePurchase % amount != 0)
            {
                amount = _elevator.Model.PricePurchase % amount;
            }
            if (_gameManager.Model.Cash < amount)
            {
                amount = _gameManager.Model.Cash;
            }
            _gameManager.Model.Cash -= amount;
            _gameManager.Model.SetChanged();
            _gameManager.Model.Save();

            _elevator.Model.PricePurchase -= amount;
            _elevator.Model.SetChanged();

            _gameManager.FireFlyToRemoveCash(_elevator.View.HudView.transform.position);

            if (_elevator.Model.PricePurchase > 0) return;

            _gameManager.Model.SavePlaceIsPurchased(_elevator.Model.ID);
            _elevator.Model.IsPurchased = _gameManager.Model.LoadPlaceIsPurchased(_elevator.Model.ID);
            _elevator.Model.SetChanged();

            _gameManager.FireElevatorPurchased();

            _elevator.SwitchToState(new ElevatorPurchasedState());
            _gameManager.Player.SwitchToState(new PlayerPauseState());
        }
    }
}

