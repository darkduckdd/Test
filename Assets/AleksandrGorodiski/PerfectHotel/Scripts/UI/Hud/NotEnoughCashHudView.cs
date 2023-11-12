using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Hud
{
    public sealed class NotEnoughCashHudView : BaseHud
    {
        public Action CLICK_CLOSE;
        public Action CLICK_ADD_CASH;

        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _getCashButton;

        protected override void OnEnable()
        {
            _closeButton.onClick.AddListener(OnCloseButtonClick);
            _getCashButton.onClick.AddListener(OnAddCashButtonClick);
        }

        protected override void OnDisable()
        {
            _closeButton.onClick.RemoveListener(OnCloseButtonClick);
            _getCashButton.onClick.RemoveListener(OnAddCashButtonClick);
        }

        void OnCloseButtonClick()
        {
            CLICK_CLOSE.SafeInvoke();
        }

        void OnAddCashButtonClick()
        {
            CLICK_ADD_CASH.SafeInvoke();
        }
    }
}