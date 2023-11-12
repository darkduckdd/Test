public class CorpsmanInitializeState : CorpsmanState
{
    public override void Initialize()
    {
        _corpsman.InitialPosition = _corpsman.View.UnitView.transform.position;

        var area = _gameManager.FindArea(_corpsman.Model.Area);
        if (_corpsman.Model.IsPurchased && area.Model.IsPurchased)
            _corpsman.SwitchToState(new CorpsmanIdleState());
        else
        {
            if (_corpsman.IsPurchasable(area.Model.IsPurchased, _gameManager.Model.LoadProgress(), _corpsman.Model.TargetPurchaseValue))
            {
                _corpsman.SwitchToState(new CorpsmanReadyToPurchaseState());
            }
            else
            {
                _corpsman.SwitchToState(new CorpsmanHiddenState());
            }
        }
    }

    public override void Dispose()
    {
    }
}
