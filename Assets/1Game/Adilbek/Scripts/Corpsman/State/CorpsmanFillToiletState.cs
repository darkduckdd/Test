using UnityEngine;

public class CorpsmanFillToiletState : CorpsmanUpdateState
{
    private ItemController _item;

    public CorpsmanFillToiletState(ItemController item)
    {
        _item = item;
    }

    public override void Initialize()
    {
        base.Initialize();

        _corpsman.View.UnitView.NavMeshAgent.enabled = false;
        _corpsman.View.UnitView.Clean();

        _timer.TICK += OnTick;


    }

    public override void Dispose()
    {
        base.Dispose();

        _timer.TICK -= OnTick;
    }

    private void OnTick()
    {
        _item.Model.Duration -= Time.deltaTime;
        _item.Model.SetChanged();

        if (_item.Model.Duration > 0f) return;
        _corpsman.Model.RemoveItem();
        _item.FireItemFinished();
        _corpsman.View.ToiletPaper.SetActive(false);

        _corpsman.SwitchToState(new CorpsmanWalkHomeState(_corpsman.InitialPosition));
    }
}
