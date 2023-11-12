using Game.Core;
using Game.UI;
using Injection;

namespace Game.Level.Cleaner.CleanerState
{
    public abstract class CleanerState : State
    {
        [Inject] protected CleanerController _cleaner;
        [Inject] protected Timer _timer;
        [Inject] protected GameManager _gameManager;
        [Inject] protected GameView _gameView;

        public override void Dispose()
        {
        }

        public override void Initialize()
        {
        }

        internal void FindUsedItem()
        {
            var item = _gameManager.FindUsedItemForCleaner(_cleaner.Model.Area);
            if (item == null) return;

            _gameManager.RemoveItem(item);
            _cleaner.SwitchToState(new CleanerWalkToItemState(item));
        }
    }
}


