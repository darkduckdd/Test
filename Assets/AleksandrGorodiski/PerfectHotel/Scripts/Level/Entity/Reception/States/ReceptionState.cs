using Game.Core;
using Injection;

namespace Game.Level.Reception.ReceptionState
{
    public abstract class ReceptionState : State
    {
        [Inject] protected ReceptionController _reception;
    }
}

