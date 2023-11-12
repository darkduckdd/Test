using System.Collections.Generic;
using Game.UI.Hud;
using UnityEngine;

namespace Game.UI
{
    public sealed class GameView : MonoBehaviour
    {
        [SerializeField] public GameObject Player;
        [SerializeField] public BaseHud[] Huds;
        [SerializeField] public Joystick Joystick;
        [SerializeField] public CameraController CameraController;

        public IEnumerable<IHud> AllHuds()
        {
            foreach (var hud in Huds)
            {
                yield return hud;
            }
        }
    }
}