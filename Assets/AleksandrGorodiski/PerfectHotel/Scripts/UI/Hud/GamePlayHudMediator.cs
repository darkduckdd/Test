using System;
using Game.Core.UI;
using Game.Level;
using Game.Managers;
using Injection;

namespace Game.UI.Hud
{
    public sealed class GamePlayHudMediator : Mediator<GamePlayHudView>
    {
        [Inject] private LevelView _levelView;
        [Inject] private GameManager _gameManager;
        [Inject] private HudManager _hudManager;

        protected override void Show()
        {
            _view.DeveloperButton.gameObject.SetActive(GameConstants.IsDeveloperDevice());

            _view.MaxLevels = _levelView.MaxLevels;

            UpdateMaxProgress(_gameManager.Model.LoadLvl());
            HotelsButtonVisibility();

            _view.Model = _gameManager.Model;

            _view.DeveloperButton.onClick.AddListener(OnDeveloperButtonClick);
            _view.HotelsButton.onClick.AddListener(OnHotelsButtonClick);

            _gameManager.LEVEL_CHANGED += UpdateMaxProgress;
            _gameManager.ELEVATOR_PURCHASED += HotelsButtonVisibility;
        }

        private void HotelsButtonVisibility()
        {
            string elevatorID = _gameManager.Model.GenerateEntityID(1, EntityType.Elevator, 2);
            bool isPurchased = _gameManager.Model.LoadPlaceIsPurchased(elevatorID);

            _view.HotelsButton.gameObject.SetActive(isPurchased);
        }

        protected override void Hide()
        {
            _view.DeveloperButton.onClick.RemoveListener(OnDeveloperButtonClick);
            _view.HotelsButton.onClick.RemoveListener(OnHotelsButtonClick);

            _gameManager.LEVEL_CHANGED -= UpdateMaxProgress;
            _gameManager.ELEVATOR_PURCHASED -= HotelsButtonVisibility;
        }

        private void UpdateMaxProgress(int lvl)
        {
            _view.MaxProgress = _levelView.MaxProgress(lvl);
        }

        private void OnDeveloperButtonClick()
        {
            _hudManager.ShowAdditional<DeveloperHudMediator>();
        }

        private void OnHotelsButtonClick()
        {
            _hudManager.ShowAdditional<HotelsHudMediator>();
        }
    }
}