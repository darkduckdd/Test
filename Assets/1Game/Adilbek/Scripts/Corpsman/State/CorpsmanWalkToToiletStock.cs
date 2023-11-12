using UnityEngine;

public class CorpsmanWalkToToiletStock : CorpsmanWalkState
{
    private ItemController _item;
    private static Vector3 position;
    public CorpsmanWalkToToiletStock(ItemController item) : base(position)
    {
        _item = item;
        _endPosition = item.Transform.position;
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Dispose()
    {
        base.Dispose();
    }
    public override void OnReachDistance()
    {
        _corpsman.SwitchToState(new CorpsmanFillingState(_item));
    }

}
