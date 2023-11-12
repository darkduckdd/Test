using Injection;
using Game.Core;
using UnityEngine;

namespace Game.Level.Inventory.States
{
    public sealed class InventoryFlyState : InventoryState
    {
        [Inject] private Timer _timer;

        private Vector3 _endPosition;
        private Vector3 _startPosition;

        private float _flyTime;
        private float _timeElapsed;

        public InventoryFlyState(Vector3 endPosition, float flyTime)
        {
            _endPosition = endPosition;
            _flyTime = flyTime;
        }

        public override void Initialize()
        {
            _startPosition = _inventory.View.transform.position;

            _inventory.View.ShowTrail();

            _timer.TICK += OnTICK;
        }

        public override void Dispose()
        {
            _timer.TICK -= OnTICK;
        }

        private void OnTICK()
        {
            _inventory.View.transform.position = Vector3.Lerp(_startPosition, _endPosition, _timeElapsed / _flyTime);
            _timeElapsed += Time.deltaTime;

            float distance = Vector3.Distance(_inventory.View.transform.position, _endPosition);
            if (distance > 0.05f) return;

            _inventory.View.transform.position = _endPosition;

            _inventory.View.HideTrail();

            _inventory.FireRemoveInventory();
        }
    }
}