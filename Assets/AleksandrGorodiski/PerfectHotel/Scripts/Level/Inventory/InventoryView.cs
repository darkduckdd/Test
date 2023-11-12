using UnityEngine;

namespace Game.Level.Inventory
{
    public class InventoryView : MonoBehaviour
    {
        [SerializeField] private TrailRenderer _trail;

        public void ShowTrail()
        {
            _trail.gameObject.SetActive(true);
        }

        public void HideTrail()
        {
            _trail.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            if (null == _trail)
                return;

            _trail.Clear();
        }
    }
}