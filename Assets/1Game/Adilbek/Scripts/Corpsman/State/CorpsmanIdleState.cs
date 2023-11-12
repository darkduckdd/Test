using UnityEngine;

public class CorpsmanIdleState : CorpsmanUpdateState
{
    public override void Initialize()
    {
        base.Initialize();

        _corpsman.View.UnitView.transform.eulerAngles = new Vector3(0f, 180f, 0f);

        _corpsman.View.UnitView.Unhide();
        _corpsman.View.UnitView.NavMeshAgent.enabled = false;
        _corpsman.View.UnitView.Idle();

        _gameManager.ITEM_ADDED += FindUsedItem;

        FindUsedItem();
    }

    public override void Dispose()
    {
        base.Dispose();

        _gameManager.ITEM_ADDED -= FindUsedItem;
    }
}
