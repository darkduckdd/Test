using System;
using Game.Level.Unit;
using UnityEngine;

public sealed class PlayerView : UnitView
{
    private const float _inventoryHeight = 0.45f;

    [SerializeField] private Transform _inventoryHolder;
    [SerializeField] private Transform _aimTransform;
    [SerializeField] private string _currentState;

    public Vector3 AimPosition => _aimTransform.position;
    public Vector3 InventoryPosition => _inventoryHolder.transform.position;
    public float InventoryHeight => _inventoryHeight;

    public Type CurrentState
    {
        set => _currentState = value?.Name;
    }
}