using UnityEngine;

public class CorpsmanFillingState : CorpsmanState
{
    private ItemController _item;
    private float _duration;

    public CorpsmanFillingState(ItemController item)
    {
        _item = item;
    }

    public override void Initialize()
    {
        base.Initialize();
        _duration = 1f;
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
        _duration -= Time.deltaTime;


        if (_duration > 0f) return;
        _corpsman.Model.SetItem(_item);
        _corpsman.View.ToiletPaper.SetActive(true);
        _corpsman.SwitchToState(new CorpsmanWalkHomeState(_corpsman.InitialPosition));
    }
}
