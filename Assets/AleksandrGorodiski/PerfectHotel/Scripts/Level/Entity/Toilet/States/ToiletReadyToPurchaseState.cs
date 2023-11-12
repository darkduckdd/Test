using Game.UI;
using Injection;

namespace Game.Level.Toilet.ToiletState
{
    public sealed class ToiletReadyToPurchaseState : ToiletState
    {
        [Inject] private GameView _gameView;

        public override void Initialize()
        {
            _toilet.View.HudView.gameObject.SetActive(true);
            _toilet.View.HudView.ReadyToPuchase();
            _toilet.View.MeshesHolder.SetActive(false);
            _toilet.View.HideWallsPurchasedState();

            OnLvlChanged(_gameManager.Model.LoadLvl());

            _toilet.ItemBuyUpdate.PLAYER_ON_ITEM += PlayerOnItem;

            _gameManager.AddItem(_toilet.ItemBuyUpdate);

            _gameManager.LEVEL_CHANGED += OnLvlChanged;
        }

        public override void Dispose()
        {
            _gameManager.LEVEL_CHANGED -= OnLvlChanged;
            _toilet.ItemBuyUpdate.PLAYER_ON_ITEM -= PlayerOnItem;

            _gameManager.RemoveItem(_toilet.ItemBuyUpdate);
        }

        private void OnLvlChanged(int lvl)
        {
            _toilet.View.UpdateWallsHiddenState(lvl);
        }

        private void PlayerOnItem(ItemController item)
        {

            if (!SideAdvertising.IsActive)
            {
                if (_gameManager.Model.Cash <= 0)
                {
                    SideAdvertising.ShowPanel?.Invoke(_toilet.Model.PricePurchase, IDosGames.VirtualCurrencyID.MO);

                    return;
                }
                if (_gameManager.Model.Cash < _toilet.Model.PricePurchase)
                {
                    SideAdvertising.ShowPanel?.Invoke(_toilet.Model.PricePurchase, IDosGames.VirtualCurrencyID.MO);

                    return;
                }
            }
            if (_gameManager.Model.Cash <= 0) return;
            int amount = _toilet.Model.TransferRate;

            if (_toilet.Model.PricePurchase % amount != 0)
            {
                amount = _toilet.Model.PricePurchase % amount;
            }
            if (_gameManager.Model.Cash < amount)
            {
                amount = _gameManager.Model.Cash;
            }
            _gameManager.Model.Cash -= amount;
            _gameManager.Model.SetChanged();
            _gameManager.Model.Save();

            _toilet.Model.PricePurchase -= amount;
            _toilet.Model.SetChanged();

            _gameManager.FireFlyToRemoveCash(_toilet.View.HudView.transform.position);

            if (_toilet.Model.PricePurchase > 0) return;

            _gameManager.Model.SavePlaceIsPurchased(_toilet.Model.ID);
            _toilet.Model.IsPurchased = _gameManager.Model.LoadPlaceIsPurchased(_toilet.Model.ID);
            _toilet.Model.SetChanged();

            _gameView.CameraController.SetTarget(_toilet.View.transform);
            _gameView.CameraController.ZoomIn(true);

            _gameManager.FireAddGameProgress(_gameManager.Player.View.transform.position, _toilet.Model.PurchaseProgressReward);

            _toilet.SwitchToState(new ToiletPurchasedState());
        }
    }
}