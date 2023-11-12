using Game.Level.Player.PlayerState;

public class CorpsmanUpdateState : CorpsmanState
{
    public override void Initialize()
    {
        _corpsman.View.HudView.ReadyToUpdate();

        CheckIsUpdatable(_gameManager.Model.LoadProgress());

        _corpsman.ItemBuyUpdate.PLAYER_ON_ITEM += PlayerOnItem;
        _gameManager.PROGRESS_CHANGED += CheckIsUpdatable;
    }

    public override void Dispose()
    {
        _corpsman.ItemBuyUpdate.PLAYER_ON_ITEM -= PlayerOnItem;
        _gameManager.PROGRESS_CHANGED -= CheckIsUpdatable;
    }

    private void CheckIsUpdatable(int progress)
    {
        bool isUpdatable = _corpsman.IsUpdatable(_corpsman.Model.IsMaxed, progress, _corpsman.Model.TargetUpdateProgress);
        _corpsman.View.HudView.gameObject.SetActive(isUpdatable);

        if (isUpdatable)
            _gameManager.AddItem(_corpsman.ItemBuyUpdate);
        else
            _gameManager.RemoveItem(_corpsman.ItemBuyUpdate);
    }
    private void PlayerOnItem(ItemController item)
    {
        if (!SideAdvertising.IsActive)
        {
            if (_gameManager.Model.Cash <= 0)
            {
                SideAdvertising.ShowPanel?.Invoke(_corpsman.Model.PriceNextLvl, IDosGames.VirtualCurrencyID.MO);
                return;
            }
            if (_gameManager.Model.Cash < _corpsman.Model.PriceNextLvl)
            {
                SideAdvertising.ShowPanel?.Invoke(_corpsman.Model.PriceNextLvl, IDosGames.VirtualCurrencyID.MO);
                return;
            }
        }

        if (_gameManager.Model.Cash <= 0 || !_corpsman.IsUpdatable(_corpsman.Model.IsMaxed, _gameManager.Model.LoadProgress(), _corpsman.Model.TargetUpdateProgress)) return;

        int amount = _corpsman.Model.TransferRate;
        if (_corpsman.Model.PriceNextLvl % amount != 0)
        {
            amount = _corpsman.Model.PriceNextLvl % amount;
        }

        if (_gameManager.Model.Cash < amount)
        {
            amount = _gameManager.Model.Cash;
        }
        _gameManager.Model.Cash -= amount;
        _gameManager.Model.Save();
        _gameManager.Model.SetChanged();
        _corpsman.Model.PriceNextLvl -= amount;

        _gameManager.FireFlyToRemoveCash(_corpsman.View.HudView.transform.position);

        if (_corpsman.Model.PriceNextLvl <= 0)
        {
            _corpsman.Model.Lvl++;
            _gameManager.Model.SavePlaceLvl(_corpsman.Model.ID, _corpsman.Model.Lvl);
            _corpsman.Model.UpdateModel();

            CheckIsUpdatable(_gameManager.Model.LoadProgress());
            _corpsman.UnitView.PlayUnitParticles();

            _gameView.CameraController.SetTarget(_corpsman.UnitView.transform);
            _gameView.CameraController.ZoomIn(true);

            _gameManager.Player.SwitchToState(new PlayerPauseState());
        }

        _corpsman.Model.SetChanged();
    }
}
