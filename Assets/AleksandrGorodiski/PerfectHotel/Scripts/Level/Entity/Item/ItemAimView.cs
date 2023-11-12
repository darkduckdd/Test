using UnityEngine;

public sealed class ItemAimView : ItemReusableView
{
    [SerializeField] private Transform _aimTransform;
    [SerializeField] private Transform _customerPosition;

    public Vector3 AimPosition => _aimTransform.position;
    public Vector3 CustomerPosition => _customerPosition.position;
}
