using Game.Core;
using Injection;

namespace Game.Level.Area.AreaState
{
    public abstract class AreaState : State
    {
        [Inject] protected AreaController _area;
        [Inject] protected Timer _timer;
    }
}

