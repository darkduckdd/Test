using Game.Core;
using Game.UI;
using Injection;

namespace Game.Level.Player.PlayerState
{
    public sealed class PlayerPauseState : PlayerState
    {
        [Inject] private GameView _gameView;
        [Inject] private Timer _timer;
        [Inject] private GameManager _gameManager;

        public override void Initialize()
        {
            _player.View.Idle(_gameManager.Model.Inventories.Count);

            _timer.TICK += OnTICK;
        }

        public override void Dispose()
        {
            _timer.TICK -= OnTICK;
        }

        private void OnTICK()
        {
            if (!_gameView.Joystick.IsTouched) return;

            _player.SwitchToState(new PlayerWalkState());
        }
    }
}

