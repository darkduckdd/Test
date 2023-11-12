using UnityEngine;

public sealed class CorpsmanWalkHomeState : CorpsmanWalkState
{
    public CorpsmanWalkHomeState(Vector3 position) : base(position)
    {
        _endPosition = position;
    }

    public override void Initialize()
    {
        base.Initialize();

        _gameManager.ITEM_ADDED += FindUsedItem;

        FindUsedItem();
    }

    public override void Dispose()
    {
        base.Dispose();

        _gameManager.ITEM_ADDED -= FindUsedItem;
    }

    public override void OnReachDistance()
    {
        _corpsman.SwitchToState(new CorpsmanIdleState());
    }
}
