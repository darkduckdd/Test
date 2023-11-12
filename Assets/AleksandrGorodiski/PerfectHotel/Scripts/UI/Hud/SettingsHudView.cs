using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Hud
{
    public sealed class SettingsHudView : BaseHud
    {
        [SerializeField] private Button _closeButton;

        public Button CloseButton => _closeButton;

        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
        }
    }
}