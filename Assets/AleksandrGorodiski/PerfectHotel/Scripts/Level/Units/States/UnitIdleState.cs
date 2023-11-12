namespace Game.Level.Unit.UnitState
{
    public sealed class UnitIdleState : UnitState
    {
        public override void Initialize()
        {
            _unit.View.NavMeshAgent.enabled = false;
            _unit.View.Idle();
        }

        public override void Dispose()
        {
        }
    }
}