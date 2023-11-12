using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAnimation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private bool _depensOnInteactability = true;
    [SerializeField]
    private Button _buttonCached;
    private bool _isButtonIntaractable
    {
        get
        {
            if (_buttonCached == null)
            {
                _buttonCached = GetComponent<Button>();
            }
            if (_buttonCached == null)
            {
                return false || !_depensOnInteactability;
            }
            return _buttonCached.interactable;
        }
    }
    [SerializeField]
    private float _animationDurationIn = .05f;
    [SerializeField]
    private float _animationDurationOut = .15f;
    [SerializeField]
    private float _toScaleIn = .9f;
    [SerializeField]
    private float _amplitude = 1f;
    private RectTransform _rectTransform;

    void Awake()
    {
        _rectTransform = transform as RectTransform;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_isButtonIntaractable) return;
        PlayScaleIn();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_isButtonIntaractable) return;
        PlayScaleOut();
    }

    public void PlayScaleIn()
    {
        _rectTransform?.DOKill();
        _rectTransform?.DOScale(Vector3.one * _toScaleIn, _animationDurationIn).SetEase(Ease.InOutQuad);
    }

    public void PlayScaleOut()
    {
        _rectTransform?.DOKill();
        _rectTransform?.DOScale(Vector3.one, _animationDurationOut).SetEase(Ease.OutBack, _amplitude);
    }
}
