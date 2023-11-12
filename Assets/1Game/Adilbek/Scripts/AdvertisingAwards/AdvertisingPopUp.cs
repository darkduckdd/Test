using Game;
using IDosGames;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class AdvertisingPopUp : PopUp
{
    [SerializeField] private Button _button;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private Image _image;

    private GameManager _gameManager;

    private int _amount = 0;

    private void OnEnable()
    {
        _button.onClick.AddListener(TryToFreeSpin);
        //_button.onClick.AddListener(testGetCash);
    }
    private void OnDisable()
    {
        _button.onClick.RemoveAllListeners();
    }

    public void TryToFreeSpin()
    {

        if (AdMediation.Instance.ShowRewardedVideo(GetCash))
        {
            Debug.Log("Show rewarded video.");
        }
        else
        {
            Message.Show("Ad is not ready.");
        }
    }


    private void GetCash(bool adCompleted = true)
    {
        _gameManager.Model.Cash += _amount;
        _gameManager.Model.Save();
        _gameManager.Model.SetChanged();
        PeriodicUpdate.UploadCash?.Invoke();
        ClosePanel();
        SideAdvertising.ClosePanel?.Invoke();

    }


    public void TurnOnPanel(GameManager gameManager, int amount, Sprite sprite, string text)
    {
        _gameManager = gameManager;
        _amount = amount;
        _image.sprite = sprite;

        _text.text = $"{text} : " + MathUtil.NiceCash(_amount);
        gameObject.SetActive(true);
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }

}
