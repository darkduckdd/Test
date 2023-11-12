using UnityEngine;

namespace Game.Level.Cleaner.CleanerState
{
    public sealed class CleanerIdleState : CleanerUpdateState
    {
        public override void Initialize()
        {
            base.Initialize();

            _cleaner.View.UnitView.transform.eulerAngles = new Vector3(0f, 180f, 0f);

            _cleaner.View.UnitView.Unhide();
            _cleaner.View.UnitView.NavMeshAgent.enabled = false;
            _cleaner.View.UnitView.Idle();

            _gameManager.ITEM_ADDED += FindUsedItem;

            FindUsedItem();
        }

        public override void Dispose()
        {
            base.Dispose();

            _gameManager.ITEM_ADDED -= FindUsedItem;
        }
    }
}


