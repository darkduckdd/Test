using Game.Config;
using System;
using UnityEngine;


[Serializable]
[CreateAssetMenu(menuName = "config/corpsmanconfig")]
public sealed class CorpsmanConfig : EntityConfig
{
    public int PricePurchase;
    public int TargetPurchaseProgress;
    public int TransferRate = 10;
    public CorpsmanLvlConfig[] Lvls;
}

[Serializable]
public sealed class CorpsmanLvlConfig
{
    public int TargetUpdateProgress;
    public int Price;
    public float Speed;
}
