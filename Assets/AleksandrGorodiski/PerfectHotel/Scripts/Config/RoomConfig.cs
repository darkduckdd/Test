using System;
using UnityEngine;

namespace Game.Config
{
    public class EntityConfig : ScriptableObject
    {
        [Min(0)] public int Number;
        [Min(1)] public int Area;
    }

    [Serializable]
    [CreateAssetMenu(menuName = "config/roomconfig")]
    public sealed class RoomConfig : EntityConfig
    {
        public int PricePurchase;
        [Min(0)] public int TargetPurchaseProgress;
        public int PurchaseProgressReward;
        public int UpdateProgressReward;
        public int StayFee = 2;
        public float StayDuration = 3f;
        public int Limit = 150;
        public int TransferRate = 10;
        public RoomLvlConfig[] Lvls;
    }

    [Serializable]
    public sealed class RoomLvlConfig
    {
        public int TargetUpdateProgress;
        public float CleaningTime = 1f;
        public int Price = 50;
    }
}