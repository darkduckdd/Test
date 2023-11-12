using Game.Core.UI;
using Game.Managers;
using Injection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.UI.Hud
{
    public sealed class DeveloperHudMediator : Mediator<DeveloperHudView>
    {
        private const int _cash = 1000;

        [Inject] private HudManager _hudManager;
        [Inject] private GameManager _gameManager;
        [Inject] private GameView _gameView;
        [Inject] private LevelView _levelView;

        private bool _isDeveloperDevice;

        protected override void Show()
        {
            _gameView.Joystick.enabled = false;
            _isDeveloperDevice = GameConstants.IsDeveloperDevice();

            _view.AddCashButton.gameObject.SetActive(_isDeveloperDevice);
            _view.ResetButton.gameObject.SetActive(_isDeveloperDevice);

            _view.SetProgressSliderLimits(0, _levelView.MaxProgress(_gameManager.Model.LoadLvl()));
            _view.LoadProgress(_gameManager.Model.LoadProgress());

            _view.SetLvlSliderLimits(1, _levelView.MaxLevels);
            _view.LoadLvl(_gameManager.Model.LoadLvl());

            _view.SAVE += Save;

            _view.CloseButton.onClick.AddListener(OnCloseButtonClick);
            _view.AddCashButton.onClick.AddListener(OnAddCashButtonClick);
            _view.ResetButton.onClick.AddListener(OnResetButtonClick);
            _view.LoadGameplayButton.onClick.AddListener(OnLoadGameplayButtonClick);
        }

        private void OnCloseButtonClick()
        {
            _hudManager.HideAdditional<DeveloperHudMediator>();
        }

        protected override void Hide()
        {
            _gameView.Joystick.enabled = true;

            _view.SAVE -= Save;

            _view.CloseButton.onClick.RemoveListener(OnCloseButtonClick);
            _view.AddCashButton.onClick.RemoveListener(OnAddCashButtonClick);
            _view.ResetButton.onClick.RemoveListener(OnResetButtonClick);
            _view.LoadGameplayButton.onClick.RemoveListener(OnLoadGameplayButtonClick);
        }

        private void OnLoadGameplayButtonClick()
        {
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }

        private void Save()
        {
            _gameManager.Model.SaveProgress(_view.Progress);
            _gameManager.Model.SaveLvl(_view.Lvl);
            _gameManager.Model.Save();
            _gameManager.Model.SetChanged();

            _gameManager.FireProgressChanged(_gameManager.Model.LoadProgress());
            _gameManager.FireLevelChanged(_gameManager.Model.LoadLvl());
        }

        private void OnAddCashButtonClick()
        {
            _gameManager.Model.Cash += _cash;
            _gameManager.Model.Save();
            _gameManager.Model.SetChanged();
        }

        private void OnResetButtonClick()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();

            OnLoadGameplayButtonClick();
        }
    }
}


