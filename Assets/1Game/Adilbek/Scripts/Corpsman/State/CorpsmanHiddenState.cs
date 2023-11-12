public class CorpsmanHiddenState : CorpsmanState
{
    public override void Initialize()
    {
        _corpsman.View.UnitView.gameObject.SetActive(false);
        _corpsman.View.HudView.gameObject.SetActive(false);

        _gameManager.PROGRESS_CHANGED += OnProgressChanged;
        _gameManager.AREA_PURCHASED += OnAreaPurchased;
    }

    public override void Dispose()
    {
        _gameManager.PROGRESS_CHANGED -= OnProgressChanged;
        _gameManager.AREA_PURCHASED -= OnAreaPurchased;
    }

    private void OnProgressChanged(int progress)
    {
        var area = _gameManager.FindArea(_corpsman.Model.Area);
        CheckIsPurchasable(area, progress);
    }

    private void OnAreaPurchased(AreaController area)
    {
        if (area.Number != _corpsman.Model.Area) return;
        CheckIsPurchasable(area, _gameManager.Model.LoadProgress());
    }

    private void CheckIsPurchasable(AreaController area, int progress)
    {
        if (_corpsman.IsPurchasable(area.Model.IsPurchased, progress, _corpsman.Model.TargetPurchaseValue))
            _corpsman.SwitchToState(new CorpsmanReadyToPurchaseState());
    }
}

