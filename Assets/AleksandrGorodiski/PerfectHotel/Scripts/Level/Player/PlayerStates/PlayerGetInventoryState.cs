namespace Game.Level.Player.PlayerState
{
    public sealed class PlayerGetInventoryState : PlayerItemState
    {
        public PlayerGetInventoryState(ItemController item) : base(item)
        {
            _item = item;
        }

        public override void Initialize()
        {
            _player.View.Throw();

            base.Initialize();

            int index = _gameManager.Model.Inventories.Count + 1;

            ItemUtilityController utility = _item as ItemUtilityController;

            _gameManager.FireFlyInventoryItem(InventoryType.ToiletPaper, utility.View.AimPosition, _gameManager.Player.GetInventoryPosition(index), _item.Model.Duration);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}

