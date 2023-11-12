using Game.Core;
using Injection;

namespace Game.Level.Toilet.ToiletState
{
    public abstract class ToiletState : State
    {
        [Inject] protected ToiletController _toilet;
        [Inject] protected Timer _timer;
        [Inject] protected GameManager _gameManager;

        public override void Dispose()
        {
        }

        public override void Initialize()
        {
        }
    }
}


