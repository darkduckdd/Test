using UnityEngine;

public abstract class CorpsmanWalkState : CorpsmanUpdateState
{
    public Vector3 _endPosition;

    public CorpsmanWalkState(Vector3 position)
    {
        _endPosition = position;
    }

    public override void Initialize()
    {
        base.Initialize();

        _corpsman.View.UnitView.Walk();

        _corpsman.View.UnitView.NavMeshAgent.enabled = true;
        _corpsman.View.UnitView.NavMeshAgent.SetDestination(_endPosition);
        _corpsman.View.UnitView.NavMeshAgent.speed = _corpsman.Model.Speed;

        _timer.TICK += OnTick;
    }

    private void OnTick()
    {
        if (Vector3.Distance(_corpsman.View.UnitView.transform.position, _endPosition) > 0.05f) return;

        OnReachDistance();
    }

    public abstract void OnReachDistance();

    public override void Dispose()
    {
        base.Dispose();

        _timer.TICK -= OnTick;
    }
}
