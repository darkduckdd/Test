using Injection;
using UnityEngine;

namespace Game.Level.Unit.UnitState
{
    public sealed class UnitInToiletCabineState : UnitState
    {
        private const float _angle = 180f;

        [Inject] private LevelView _levelView;

        private ItemToiletController _cabine;
        private float _toiletDuration;

        public UnitInToiletCabineState(ItemToiletController cabine)
        {
            _cabine = cabine;
        }

        public override void Initialize()
        {
            _toiletDuration = _cabine.StayDuration;

            _cabine.View.CloseDoor();

            _unit.View.NavMeshAgent.enabled = false;
            _unit.View.Toilet();
            _unit.View.transform.eulerAngles = new Vector3(0f, _angle, 0f);

            _timer.TICK += OnTick;
        }

        public override void Dispose()
        {
            _timer.TICK -= OnTick;
        }

        private void OnTick()
        {
            _toiletDuration -= Time.deltaTime;

            if (_toiletDuration > 0f) return;

            _cabine.View.OpenDoor();
            _cabine.View.CloseDoorWithDelay();

            _cabine.FireUnitLeftToiletCabine();

            _unit.SwitchToState(new UnitWalkToRemoveState(_levelView.UnitRemovePoint.transform.position));
        }
    }
}