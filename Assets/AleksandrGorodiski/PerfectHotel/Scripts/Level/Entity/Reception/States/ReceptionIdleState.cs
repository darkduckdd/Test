using Game.Level.Player.PlayerState;
using Game.UI;
using Injection;

namespace Game.Level.Reception.ReceptionState
{
    public sealed class ReceptionIdleState : ReceptionState
    {
        [Inject] private GameManager _gameManager;
        [Inject] private GameView _gameView;

        public override void Initialize()
        {
            CheckIsUpdatable(_gameManager.Model.LoadProgress());

            _reception.ItemBuyUpdate.PLAYER_ON_ITEM += PlayerOnItem;
            _gameManager.PROGRESS_CHANGED += CheckIsUpdatable;
        }

        public override void Dispose()
        {
            _reception.ItemBuyUpdate.PLAYER_ON_ITEM -= PlayerOnItem;
            _gameManager.PROGRESS_CHANGED -= CheckIsUpdatable;
        }

        private void CheckIsUpdatable(int progress)
        {
            bool isUpdatable = _reception.IsUpdatable(_reception.Model.IsMaxed, progress, _reception.Model.TargetUpdateProgress);
            _reception.View.HudView.gameObject.SetActive(isUpdatable);

            if (isUpdatable)
            {
                _gameManager.AddItem(_reception.ItemBuyUpdate);
                _reception.View.HudView.ReadyToUpdate();
            }
            else _gameManager.RemoveItem(_reception.ItemBuyUpdate);
        }

        private void PlayerOnItem(ItemController item)
        {
            if (!SideAdvertising.IsActive)
            {
                if (_gameManager.Model.Cash <= 0)
                {
                    SideAdvertising.ShowPanel?.Invoke(_reception.Model.PriceNextLvl, IDosGames.VirtualCurrencyID.MO);
                    return;
                }
                if (_gameManager.Model.Cash < _reception.Model.PriceNextLvl)
                {
                    SideAdvertising.ShowPanel?.Invoke(_reception.Model.PriceNextLvl, IDosGames.VirtualCurrencyID.MO);

                    return;
                }
            }

            if (_gameManager.Model.Cash <= 0 || !_reception.IsUpdatable(_reception.Model.IsMaxed, _gameManager.Model.LoadProgress(), _reception.Model.TargetUpdateProgress)) return;
            int amount = _reception.Model.TransferRate;

            if (_reception.Model.PriceNextLvl % amount != 0)
            {
                amount = _reception.Model.PriceNextLvl % amount;
            }

            if (_gameManager.Model.Cash < amount)
            {
                amount = _gameManager.Model.Cash;
            }
            _gameManager.Model.Cash -= amount;
            _gameManager.Model.Save();
            _gameManager.Model.SetChanged();

            _reception.Model.PriceNextLvl -= amount;

            _gameManager.FireFlyToRemoveCash(_reception.View.HudView.transform.position);

            if (_reception.Model.PriceNextLvl <= 0)
            {
                _reception.Model.Lvl++;
                _gameManager.Model.SavePlaceLvl(_reception.Model.ID, _reception.Model.Lvl);
                _reception.Model.UpdateModel();

                CheckIsUpdatable(_gameManager.Model.LoadProgress());

                _gameView.CameraController.SetTarget(_reception.View.transform);
                _gameView.CameraController.ZoomIn(true);

                _gameManager.Player.SwitchToState(new PlayerPauseState());
            }

            _reception.Model.SetChanged();
        }
    }
}


