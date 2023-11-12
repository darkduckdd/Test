namespace Game.Level.Player.PlayerState
{
    public sealed class PlayerIdleState : PlayerFindEntityState
    {
        public override void Initialize()
        {
            base.Initialize();

            _player.View.NavMeshAgent.enabled = false;

            _player.View.Idle(_gameManager.Model.Inventories.Count);

            _timer.TICK += OnTICK;
        }

        public override void Dispose()
        {
            base.Dispose();

            _timer.TICK -= OnTICK;
        }

        internal void FindClosestUsedItem()
        {
            var item = _gameManager.FindClosestUsedItem();
            if (item == null) return;

            var type = item.Type;
            var inventory = GetInventory(type);

            if (type == ItemType.Clean)
                _player.SwitchToState(new PlayerCleaningState(item));

            else if (type == ItemType.ReceptionDesk)
                _player.SwitchToState(new PlayerReceptionState(item));

            else if (type == ItemType.CashPile || type == ItemType.BuyUpdate)
                _player.SwitchToState(new PlayerOnItemState(item));

            else if (type == ItemType.ToiletCabine && HasInventory(inventory))
                _player.SwitchToState(new PlayerPassInventoryState(item, inventory));

            else if (type == ItemType.UtilityToiletPaper && _gameManager.Model.Inventories.Count < _config.InventoryMax)
                _player.SwitchToState(new PlayerGetInventoryState(item));

            else if (type == ItemType.ShowHud)
                _player.SwitchToState(new PlayerElevatorState(item));

            else Log.Warning("Not implemented ItemType: " + type);
        }

        private void OnTICK()
        {
            if (!_gameView.Joystick.IsTouched) return;

            _player.SwitchToState(new PlayerWalkState());
        }

        public override void OnSecondTick()
        {
            base.OnSecondTick();

            FindClosestUsedItem();
        }

        private InventoryType GetInventory(ItemType type)
        {
            InventoryType result = InventoryType.None;

            if (type == ItemType.ToiletCabine)
                result = InventoryType.ToiletPaper;

            return result;
        }

        private bool HasInventory(InventoryType inventory)
        {
            return _gameManager.Model.Inventories.Contains(inventory);
        }
    }
}