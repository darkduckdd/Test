namespace Game.Level.Player.PlayerState
{
    public sealed class PlayerReceptionState : PlayerItemState
    {
        public PlayerReceptionState(ItemController item) : base(item)
        {
            _item = item;
        }

        public override void Initialize()
        {
            _player.View.Reception();

            base.Initialize();
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}

