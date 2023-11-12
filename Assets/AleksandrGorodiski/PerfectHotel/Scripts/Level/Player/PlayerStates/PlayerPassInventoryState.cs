namespace Game.Level.Player.PlayerState
{
    public sealed class PlayerPassInventoryState : PlayerItemState
    {
        private InventoryType _type;

        public PlayerPassInventoryState(ItemController item, InventoryType type) : base(item)
        {
            _item = item;
            _type = type;
        }

        public override void Initialize()
        {
            _player.View.Throw();

            base.Initialize();

            int index = _gameManager.Model.Inventories.Count;

            ItemToiletController cabine = _item as ItemToiletController;

            _gameManager.FireFlyInventoryItem(_type, _gameManager.Player.GetInventoryPosition(index), cabine.View.AimPosition, _item.Model.Duration);

            _gameManager.Model.Inventories.Remove(_type);
            _gameManager.Model.Save();
            _gameManager.Model.SetChanged();
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}

