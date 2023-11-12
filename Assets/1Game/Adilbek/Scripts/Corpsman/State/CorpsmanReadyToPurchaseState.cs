using Game.Level.Player.PlayerState;

public class CorpsmanReadyToPurchaseState : CorpsmanState
{
    public override void Initialize()
    {
        _corpsman.View.HudView.gameObject.SetActive(true);
        _corpsman.View.HudView.ReadyToPuchase();
        _corpsman.View.UnitView.Hide();

        _corpsman.ItemBuyUpdate.PLAYER_ON_ITEM += PlayerOnItem;

        _gameManager.AddItem(_corpsman.ItemBuyUpdate);
    }

    public override void Dispose()
    {
        _corpsman.ItemBuyUpdate.PLAYER_ON_ITEM -= PlayerOnItem;

        _gameManager.RemoveItem(_corpsman.ItemBuyUpdate);
    }

    private void PlayerOnItem(ItemController item)
    {

        if (!SideAdvertising.IsActive)
        {
            if (_gameManager.Model.Cash <= 0)
            {
                SideAdvertising.ShowPanel?.Invoke(_corpsman.Model.PricePurchase, IDosGames.VirtualCurrencyID.MO);

                return;
            }
            if (_gameManager.Model.Cash < _corpsman.Model.PricePurchase)
            {
                SideAdvertising.ShowPanel?.Invoke(_corpsman.Model.PricePurchase, IDosGames.VirtualCurrencyID.MO);

                return;
            }
        }

        if (_gameManager.Model.Cash <= 0) return;

        int amount = _corpsman.Model.TransferRate;
        if (_corpsman.Model.PricePurchase % amount != 0)
        {
            amount = _corpsman.Model.PricePurchase % amount;
        }

        if (_gameManager.Model.Cash < amount)
        {
            amount = _gameManager.Model.Cash;
        }

        _gameManager.Model.Cash -= amount;
        _gameManager.Model.SetChanged();
        _gameManager.Model.Save();

        _corpsman.Model.PricePurchase -= amount;
        _corpsman.Model.SetChanged();

        _gameManager.FireFlyToRemoveCash(_corpsman.View.HudView.transform.position);

        if (_corpsman.Model.PricePurchase > 0) return;

        _gameManager.Model.SavePlaceIsPurchased(_corpsman.Model.ID);
        _corpsman.Model.IsPurchased = _gameManager.Model.LoadPlaceIsPurchased(_corpsman.Model.ID);
        _corpsman.Model.SetChanged();

        _corpsman.UnitView.PlayUnitParticles();

        _gameView.CameraController.SetTarget(_corpsman.UnitView.transform);
        _gameView.CameraController.ZoomIn(true);

        _corpsman.SwitchToState(new CorpsmanIdleState());
        _gameManager.Player.SwitchToState(new PlayerPauseState());
    }
}

