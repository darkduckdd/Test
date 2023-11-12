using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private const float _maxRadius = 85f;

    [SerializeField] private GameObject _background, _handle;

    [HideInInspector] public bool IsTouched;
    [HideInInspector] public float Horizontal, Vertical;

    private Vector2 _touchPosition;
    private Touch _oneTouch;

    void Start()
    {
        _background.SetActive(false);
        _handle.SetActive(false);
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
#if UNITY_EDITOR
        if (eventData.IsPointerMoving())
        {
            _touchPosition = Input.mousePosition;
            if (!IsTouched)
            {
                IsTouched = true;
                _background.SetActive(true);
                _handle.SetActive(true);
                _background.transform.position = _touchPosition;
                _handle.transform.position = _touchPosition;
            }
            Move();
        }
#else
        if (eventData.IsPointerMoving() && eventData.pointerId == 0)
        {
            _oneTouch = Input.GetTouch(0);
            _touchPosition = _oneTouch.position;

            if (!IsTouched)
            {
                IsTouched = true;
                _background.SetActive(true);
                _handle.SetActive(true);
                _background.transform.position = _touchPosition;
                _handle.transform.position = _touchPosition;
            }
            Move();
        }
#endif
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        IsTouched = false;
        _background.SetActive(false);
        _handle.SetActive(false);
    }

    private void Move()
    {
        _handle.transform.position = _touchPosition;
        _handle.transform.localPosition = Vector2.ClampMagnitude(_handle.transform.localPosition, _maxRadius);
        Horizontal = _handle.transform.localPosition.x / _maxRadius;
        Vertical = _handle.transform.localPosition.y / _maxRadius;
    }
}


