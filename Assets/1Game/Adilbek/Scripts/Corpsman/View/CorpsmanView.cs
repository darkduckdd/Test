using UnityEngine;

public sealed class CorpsmanView : EntityWithHudView
{
    [SerializeField] private CorpsmanConfig _config;
    [SerializeField] private CorpsmanUnitView _unitView;
    [SerializeField] private ItemView _itemBuyUpdateView;
    [SerializeField] private GameObject _toiletPaper;

    public CorpsmanConfig Config => _config;
    public CorpsmanUnitView UnitView => _unitView;
    public ItemView ItemBuyUpdateView => _itemBuyUpdateView;
    public GameObject ToiletPaper => _toiletPaper;
}