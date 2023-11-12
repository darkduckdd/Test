using Game.Managers;
using Game.UI.Hud;
using Injection;

namespace Game.Level.Player.PlayerState
{
    public sealed class PlayerElevatorState : PlayerItemState
    {
        [Inject] private HudManager _hudManager;

        public PlayerElevatorState(ItemController item) : base(item)
        {
            _item = item;
        }

        public override void Initialize()
        {
            _hudManager.ShowAdditional<HotelsHudMediator>();

            _player.View.Idle(_gameManager.Model.Inventories.Count);

            base.Initialize();
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override void OnItemFinished()
        {
        }
    }
}

