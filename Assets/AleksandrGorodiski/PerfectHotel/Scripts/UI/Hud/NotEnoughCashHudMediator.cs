using Game.Core.UI;
using Game.Managers;
using Injection;

namespace Game.UI.Hud
{
    public sealed class NotEnoughCashHudMediator : Mediator<NotEnoughCashHudView>
    {
        [Inject] private HudManager _hudManager;

        protected override void Show()
        {
            _view.CLICK_CLOSE += OnClickClose;
            _view.CLICK_ADD_CASH += OnClickAddCash;
        }

        protected override void Hide()
        {
            _view.CLICK_CLOSE -= OnClickClose;
            _view.CLICK_ADD_CASH -= OnClickAddCash;
        }

        private void OnClickClose()
        {
            InternalHide();
        }

        private void OnClickAddCash()
        {
            InternalHide();
        }
    }
}