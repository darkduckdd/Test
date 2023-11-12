using Game.Config;
using Injection;

namespace Game.States
{
    public partial class GameInitializeState : GameState
    {
        [Inject] private GameStateManager _gameStateManager;
        [Inject] private Context _context;

        public override void Initialize()
        {
            var config = GameConfig.Load();

            _context.Install(config);
            _context.ApplyInstall();

            _gameStateManager.SwitchToState(new GameLoadLevelState());
        }

        public override void Dispose()
        {
        }
    }
}