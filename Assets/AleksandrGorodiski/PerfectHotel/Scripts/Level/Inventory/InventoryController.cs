using System;
using Game.Core;
using Game.Level.Inventory.States;
using Injection;

namespace Game.Level.Inventory
{
    public sealed class InventoryController : IDisposable
    {
        public Action<InventoryController> REMOVE_INVENTORY;

        private readonly InventoryType _type;
        private readonly InventoryView _view;
        private readonly StateManager<InventoryState> _stateManager;

        public InventoryView View => _view;
        public InventoryType Type => _type;

        public InventoryController(InventoryView view, InventoryType type, Context context)
        {
            _view = view;
            _type = type;

            var subContext = new Context(context);
            var injector = new Injector(subContext);

            subContext.Install(this);
            subContext.Install(injector);

            _stateManager = new StateManager<InventoryState>();
            injector.Inject(_stateManager);
        }

        public void Dispose()
        {
            _stateManager.Dispose();
        }

        public void SwitchToState<T>(T instance) where T : InventoryState
        {
            _stateManager.SwitchToState(instance);
        }

        internal void FireRemoveInventory()
        {
            REMOVE_INVENTORY.SafeInvoke(this);
        }
    }
}

