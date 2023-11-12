using Game;
using IDosGames;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class SideAdvertising : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private AdvertisingPopUp _popup;
    [SerializeField] private Button _button;
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _label;
    [SerializeField] private List<ResourceType> _resources;

    [SerializeField] private SidePanelTimer _timer;
    [SerializeField] private float _activeTime = 10f;
    [SerializeField] private float _itemMinuteCoolDown = 0.5f;
    [Range(1, 3)][SerializeField] private float _coefficient = 1.2f;


    public static GameManager _gameManager;

    public static bool IsActive => _isActive;

    public static Action<int, VirtualCurrencyID> ShowPanel;
    public static Action ClosePanel;

    private int _amount;
    private static bool _isActive;
    private ResourceType _resourceType;
    private void Awake()
    {
        _panel.SetActive(false);
        ShowPanel += OnShowPanel;
        ClosePanel += OnClosePanel;

        _button.onClick.AddListener(ShowPopUp);

    }
    public static void SetManager(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    private void OnClosePanel()
    {
        _popup.ClosePanel();
        if (_panel.activeSelf)
        {
            StartCoroutine(PanelCoolDown());
        }
        _panel.SetActive(false);

    }

    private void OnShowPanel(int amount, VirtualCurrencyID currency)
    {
        _isActive = true;
        _panel.SetActive(true);
        _timer.UpdateTimer(_activeTime);

        ResourceType resource = _resources.Single(res => res.currency == currency);
        _resourceType = resource;
        _image.sprite = resource.sprite;
        _amount = (int)Math.Ceiling(amount * _coefficient);
        _label.text = $"{resource.lable} : {MathUtil.NiceCash((int)Math.Ceiling(amount * _coefficient))}";
    }

    private void ShowPopUp()
    {
        _popup.TurnOnPanel(_gameManager, _amount, _resourceType.sprite, _resourceType.lable);
    }

    private IEnumerator PanelCoolDown()
    {
        yield return new WaitForSeconds(_itemMinuteCoolDown * 60);
        _isActive = false;
    }
}

[Serializable]
public struct ResourceType
{
    public VirtualCurrencyID currency;
    public Sprite sprite;
    public string lable;
}
