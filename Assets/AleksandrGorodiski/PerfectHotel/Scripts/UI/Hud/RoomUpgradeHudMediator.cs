using System.Collections.Generic;
using Game.Core.UI;
using Injection;
using UnityEngine;

namespace Game.UI.Hud
{
    public class RoomUpgradeHudMediator : Mediator<RoomUpgradeHudView>
    {
        [Inject] private GameManager _gameManager;
        [Inject] private GameView _gameView;

        private RoomController _room;

        protected List<RoomSlotView> _slots;

        public RoomUpgradeHudMediator(RoomController room)
        {
            _room = room;
            _slots = new List<RoomSlotView>();
        }

        protected override void Show()
        {
            _gameView.Joystick.enabled = false;

            InsantiateSlots();

            _view.SetLvl(_room.Model.Lvl);

            _gameView.CameraController.SetTarget(_room.View.transform);
            _gameView.CameraController.ZoomIn(false);
        }

        protected override void Hide()
        {
            foreach (var slot in _slots)
            {
                slot.ON_SLOT_CLICK -= OnSlotClick;
            }

            foreach (var slot in _slots)
            {
                GameObject.Destroy(slot.gameObject);
            }
            _slots.Clear();

        }

        void OnSlotClick(int visualIndex)
        {
            _gameView.Joystick.enabled = true;

            _room.Model.VisualIndex = visualIndex;
            _gameManager.Model.SavePlaceVisualIndex(_room.Model.ID, _room.Model.VisualIndex);
            _room.Model.SetChanged();

            OnCloseButtoClick();
        }

        private void OnCloseButtoClick()
        {
            _gameManager.FireAddGameProgress(_room.View.transform.position, _room.Model.UpdateProgressReward);

            _gameView.CameraController.SetTarget(_gameManager.Player.Transform);
            _gameView.CameraController.ZoomOut();

            InternalHide();
        }

        private void InsantiateSlots()
        {
            for (int i = 0; i < _room.View.InsideWalls.Icons.Length; i++)
            {
                RoomSlotView slot = GameObject.Instantiate(_view.RoomSlotPrefab, _view.Container).GetComponent<RoomSlotView>();
                slot.Initialize(_room.View.InsideWalls.Icons[i], i);
                _slots.Add(slot);
                slot.ON_SLOT_CLICK += OnSlotClick;
            }
        }
    }
}