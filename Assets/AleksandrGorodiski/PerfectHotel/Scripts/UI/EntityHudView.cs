using Game;
using Game.UI.Hud;
using TMPro;
using UnityEngine;
using Utilities;

public class EntityHudView : BaseHudWithModel<EntityModel>
{
    [SerializeField] private GameObject _lockedIcon;
    [SerializeField] private GameObject _purchaseIcon;
    [SerializeField] private GameObject _updateIcon;
    [SerializeField] private TMP_Text _infoText;
    [SerializeField] private TMP_Text _priceText;

    private string _info;
    private string _price;

    protected override void OnEnable()
    {
    }

    protected override void OnDisable()
    {
    }

    public void Locked()
    {
        _lockedIcon.SetActive(true);
        _purchaseIcon.SetActive(false);
        _updateIcon.SetActive(false);
    }

    public void ReadyToPuchase()
    {
        _lockedIcon.SetActive(false);
        _purchaseIcon.SetActive(true);
        _updateIcon.SetActive(false);
    }

    public void ReadyToUpdate()
    {
        _lockedIcon.SetActive(false);
        _purchaseIcon.SetActive(false);
        _updateIcon.SetActive(true);
    }

    protected override void OnApplyModel(EntityModel model)
    {
        base.OnApplyModel(model);
    }

    protected override void OnModelChanged(EntityModel model)
    {
        _info = model.Type.ToString();
        _price = GameConstants.CashIconString + " " + MathUtil.NiceCash(model.PricePurchase);

        if (model.Type == EntityType.Area || model.Type == EntityType.Elevator)
        {
            if (model.IsLocked)
            {
                _info = "Level " + model.TargetPurchaseValue;
                _price = "";
            }
            else
            {
                if (model.Type == EntityType.Area)
                    _info = "New Area";
                else if (model.Type == EntityType.Elevator)
                    _info = "New Hotel";
            }
        }

        if (model.IsPurchased)
        {
            _price = GameConstants.CashIconString + " " + MathUtil.NiceCash(model.PriceNextLvl);

            int lvl = model.LvlNext + 1;
            _info = "Lvl " + lvl;

            if (model.Type == EntityType.Reception)
                _info = "Receptionist";
            else if (model.Type == EntityType.Cleaner)
                _info = "Speed";
        }

        _infoText.text = _info;
        _priceText.text = _price;
    }
}
