namespace Game.Level.Player.PlayerState
{
    public sealed class PlayerCleaningState : PlayerItemState
    {
        public PlayerCleaningState(ItemController item) : base(item)
        {
            _item = item;
        }

        public override void Initialize()
        {
            _player.View.Clean();

            base.Initialize();
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}

