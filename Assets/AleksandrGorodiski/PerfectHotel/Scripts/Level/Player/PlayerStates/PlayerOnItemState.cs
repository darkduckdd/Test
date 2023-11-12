namespace Game.Level.Player.PlayerState
{
    public sealed class PlayerOnItemState : PlayerItemState
    {
        public PlayerOnItemState(ItemController item) : base(item)
        {
            _item = item;
        }

        public override void Initialize()
        {
            _player.View.Idle(_gameManager.Model.Inventories.Count);

            base.Initialize();
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override void PlayerOnItem()
        {
            _item.FirePlayerOnItem();
        }

        public override void OnItemFinished()
        {
        }
    }
}

