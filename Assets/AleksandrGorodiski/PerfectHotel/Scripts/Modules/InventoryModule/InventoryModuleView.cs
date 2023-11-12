using System.Collections.Generic;
using UnityEngine;
using Game.UI.Pool;

public sealed class InventoryModuleView : MonoBehaviour
{
    public ComponentPoolFactory ToiletPaperPool;

    public readonly Dictionary<InventoryType, ComponentPoolFactory> InventoryMap;

    public InventoryModuleView()
    {
        InventoryMap = new Dictionary<InventoryType, ComponentPoolFactory>();
    }

    private void Awake()
    {
        InventoryMap[InventoryType.ToiletPaper] = ToiletPaperPool;
    }
}
