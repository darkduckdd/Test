using System;
using UnityEngine;

namespace Game.Config
{
    [CreateAssetMenu(menuName = "config/gameconfig")]
    public sealed class GameConfig : ScriptableObject
    {
        public static GameConfig Load()
        {
            var result = Resources.Load<GameConfig>("GameConfig");
            return result;
        }

        [Min(0)] public int DefaultCash = 1000;
        [Min(1)] public int DefaultHotel;

        public float CashPileRadius = 1f;
        public float ReceptionItemRadius = 0.75f;
        public float BuyUpdateRadius = 1f;
        public float RoomItemRadius = 0.75f;
        public float ToiletCabineRadius = 0.5f;
        public float ToiletPaperFlyTime = 0.3f;
        [Min(1)] public int ToiletVisitsCountMax;
        public float UtilityInventoryRadius = 0.5f;
        [Min(1)] public int InventoryMax;
        public float ElevatorItemRadius = 1f;
        public float EntityRadius = 3f;

        [Header("Units")]
        public float PlayerWalkSpeed = 4f;
        public float PlayerRotationSpeed = 10f;
        public float CustomerWalkSpeed = 3f;

        public GameConfig()
        {
        }

        [Header("")]
        public HotelConfig[] Hotels;

        [Header("Splash Screen")]
        public float SplashScreenDuration = 1f;
    }

    [Serializable]
    public sealed class HotelConfig
    {
        [Min(1)] public int SceneIndex;
        public Sprite Icon;
        public string Label;
    }
}