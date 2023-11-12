using Game.Core.UI;
using Game.Managers;
using Injection;
using UnityEngine.SceneManagement;

namespace Game.UI.Hud
{
    public sealed class SettingsHudMediator : Mediator<SettingsHudView>
    {
        [Inject] private HudManager _hudManager;

        protected override void Show()
        {
            _view.CloseButton.onClick.AddListener(OnCloseButtonClick);
        }

        protected override void Hide()
        {
            _view.CloseButton.onClick.RemoveListener(OnCloseButtonClick);
        }

        private void OnCloseButtonClick()
        {
            _hudManager.HideAdditional<SettingsHudMediator>();
        }
    }
}