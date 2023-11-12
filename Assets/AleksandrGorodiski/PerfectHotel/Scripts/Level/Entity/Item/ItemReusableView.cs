using UnityEngine;
using System.Collections;

public class ItemReusableView : ItemFillBarView
{
    private const float _closeDoorDelay = 2f;
    private const float _opneAngle = 110f;

    [SerializeField] private ConstructionItemView _available;
    [SerializeField] private ConstructionItemView _used;

    internal void SetVisual(bool isAvailable, int visualIndex)
    {
        if (isAvailable)
        {
            _available.MeshesVisibilityIndex(visualIndex);
            _used.HideAllMeshes();
        }
        else
        {
            _used.MeshesVisibilityIndex(visualIndex);
            _available.HideAllMeshes();
        }
    }

    internal void OpenDoor()
    {
        _available.transform.localEulerAngles = new Vector3(0f, _opneAngle, 0f);
        _used.transform.localEulerAngles = new Vector3(0f, _opneAngle, 0f);
    }

    internal void CloseDoor()
    {
        _available.transform.localEulerAngles = Vector3.zero;
        _used.transform.localEulerAngles = Vector3.zero;
    }

    internal void CloseDoorWithDelay()
    {
        StartCoroutine(WaitAndCloseDoor());
    }

    private IEnumerator WaitAndCloseDoor()
    {
        yield return new WaitForSeconds(_closeDoorDelay);
        CloseDoor();
    }
}
