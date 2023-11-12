using UnityEngine;

public sealed class ItemUtilityView : ItemView
{
    [SerializeField] private Transform _aimTransform;

    public Vector3 AimPosition => _aimTransform.position;
}
