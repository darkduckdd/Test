using Game.Config;
using UnityEngine;

public enum EntityType
{
    None,
    Reception,
    Room,
    WC,
    Cleaner,
    Utility,
    Area,
    Elevator,
    Corpsman
}

public class EntityView : MonoBehaviour
{
    [SerializeField] private EntityType _type;
    public EntityType Type => _type;
}

public class EntityWithHudView : EntityView
{
    [SerializeField] private EntityHudView _hudView;
    public EntityHudView HudView => _hudView;
}

public class PlaceView : EntityWithHudView
{
    [SerializeField] private ItemView _itemBuyUpdateView;
    [SerializeField] private GameObject _meshesHolder;

    public ItemView ItemBuyUpdateView => _itemBuyUpdateView;
    public GameObject MeshesHolder => _meshesHolder;
}

public class PlaceWithCashPileView : PlaceWithWallsView
{
    [SerializeField] private CashPileView _cashPileView;
    [SerializeField] private ItemView _itemCashPileView;

    public CashPileView CashPileView => _cashPileView;
    public ItemView ItemCashPileView => _itemCashPileView;
}

public class PlaceWithWallsView : PlaceView
{
    [SerializeField] private int _cameraAngleSign;
    [SerializeField] private ConstructionItemView _outsideWalls;
    [SerializeField] private GameObject _wallsHiddenStateHolder;
    [SerializeField] private GameObject _wallsPurchasedStateHolder;

    private ConstructionItemView[] _wallsHiddenState;
    private ConstructionItemView[] _wallsPurchasedState;

    public ConstructionItemView OutsideWalls => _outsideWalls;
    public int CameraAngleSign => _cameraAngleSign;

    public virtual void Awake()
    {
        _wallsHiddenState = _wallsHiddenStateHolder.GetComponentsInChildren<ConstructionItemView>();
        _wallsPurchasedState = _wallsPurchasedStateHolder.GetComponentsInChildren<ConstructionItemView>();
    }

    internal void HideWallsHiddenState()
    {
        foreach (var wallView in _wallsHiddenState)
        {
            wallView.HideAllMeshes();
        }
    }

    internal void UpdateWallsHiddenState(int lvl)
    {
        foreach (var wallView in _wallsHiddenState)
        {
            wallView.MeshesVisibilityLvl(lvl);
        }
    }

    internal void HideWallsPurchasedState()
    {
        foreach (var wallView in _wallsPurchasedState)
        {
            wallView.HideAllMeshes();
        }
    }

    internal void UpdateWallsPurchasedState(int lvl)
    {
        foreach (var wallView in _wallsPurchasedState)
        {
            wallView.MeshesVisibilityLvl(lvl);
        }
    }
}

public class PlaceWithItemsAimView : PlaceWithCashPileView
{
    [SerializeField] private ItemAimView[] _items;
    public ItemAimView[] Items => _items;
}

public class PlaceWithItemsReusableView : PlaceWithCashPileView
{
    [SerializeField] private ItemReusableView[] _items;
    public ItemReusableView[] Items => _items;
}

public class PlaceWithItemsFillBarView : PlaceWithCashPileView
{
    [SerializeField] private ItemFillBarView[] _items;
    public ItemFillBarView[] Items => _items;
}

public sealed class RoomView : PlaceWithItemsReusableView
{
    [SerializeField] private ConstructionInsideView _insideWalls;
    [SerializeField] private RoomConfig _config;
    [SerializeField] private GameObject _lightDark;
    [SerializeField] private Transform _customerPosition;

    public ConstructionInsideView InsideWalls => _insideWalls;
    public RoomConfig Config => _config;
    public Transform CustomerPosition => _customerPosition;

    internal void SetDarkLight(bool value)
    {
        _lightDark.SetActive(value);
    }

    public override void Awake()
    {
        base.Awake();
        _lightDark.SetActive(false);
    }
}