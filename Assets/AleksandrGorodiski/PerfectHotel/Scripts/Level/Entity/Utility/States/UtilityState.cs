using Game.Core;
using Injection;

namespace Game.Level.Utility.UtilityState
{
    public abstract class UtilityState : State
    {
        [Inject] protected UtilityController _utility;
        [Inject] protected GameManager _gameManager;

        public override void Dispose()
        {
        }

        public override void Initialize()
        {
        }
    }
}


