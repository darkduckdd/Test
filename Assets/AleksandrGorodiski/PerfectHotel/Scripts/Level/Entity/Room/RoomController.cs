using Core;
using Game;
using Game.Config;
using Game.Core;
using Game.Level.Room.RoomState;
using Injection;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityModel : Observable
{
    public EntityType Type;
    public string ID;

    public long Cash;

    public int Lvl;
    public int LvlNext;

    public bool IsPurchased { get; internal set; }
    public bool IsMaxed { get; internal set; }
    public bool IsLocked { get; internal set; }

    public int PricePurchase;
    public int PriceNextLvl;

    public int Area;
    public int TargetPurchaseValue;
    public int TargetUpdateProgress;

    public int PurchaseProgressReward;
    public int UpdateProgressReward;

    public EntityModel(string id, int lvl, EntityType type)
    {
        ID = id;
        Lvl = lvl;
        Type = type;
    }

    public abstract int GetLvlLength();

    public virtual void UpdateModel()
    {
        LvlNext = Lvl + 1;
        int lvlLength = GetLvlLength();
        if (LvlNext >= lvlLength)
        {
            LvlNext = Lvl;
            IsMaxed = true;
        }
        GetUpdatedValues();
    }
    public abstract void GetUpdatedValues();
}

public sealed class CleanerModel : EntityModel
{
    public float Speed;
    public int TransferRate;
    public CleanerLvlConfig[] Lvls;

    public CleanerModel(string id, int lvl, EntityType type, CleanerConfig config) : base(id, lvl, type)
    {
        PricePurchase = config.PricePurchase;
        Lvls = config.Lvls;
        TargetPurchaseValue = config.TargetPurchaseProgress;
        Area = config.Area;
        TransferRate = config.TransferRate;

        UpdateModel();
    }

    public override void GetUpdatedValues()
    {
        TargetUpdateProgress = Lvls[LvlNext].TargetUpdateProgress;
        PriceNextLvl = Lvls[LvlNext].Price;
        Speed = Lvls[Lvl].Speed;
    }

    public override int GetLvlLength()
    {
        return Lvls.Length;
    }
}

public sealed class ToiletModel : EntityModel
{
    public int VisualIndex;
    public float StayDuration;
    public int StayFee;
    public int TransferRate;

    public ToiletModel(string id, int lvl, EntityType type, ToiletConfig config, int visualIndex) : base(id, lvl, type)
    {
        VisualIndex = visualIndex;
        PricePurchase = config.PricePurchase;
        StayFee = config.StayFee;
        StayDuration = config.StayDuration;
        TargetPurchaseValue = config.TargetPurchaseProgress;
        PurchaseProgressReward = config.PurchaseProgressReward;
        Area = config.Area;
        TransferRate = config.TransferRate;
        UpdateModel();
    }

    public override int GetLvlLength()
    {
        return 0;
    }

    public override void GetUpdatedValues()
    {
    }
}

public sealed class ReceptionModel : EntityModel
{
    public int ReceptionistCount;
    public int EntranceFee;
    public float UnitProceedTime = 1f;
    public int Limit;
    public int TransferRate;

    public ReceptionLvlConfig[] Lvls;
    public int ItemsToShow = 1;

    public ReceptionModel(string id, int lvl, EntityType type, ReceptionConfig config) : base(id, lvl, type)
    {
        EntranceFee = config.EntranceFee;
        Lvls = config.Lvls;
        Limit = config.Limit;
        TransferRate = config.TransferRate;
        UpdateModel();
    }

    public override int GetLvlLength()
    {
        return Lvls.Length;
    }

    public override void GetUpdatedValues()
    {
        TargetUpdateProgress = Lvls[LvlNext].TargetUpdateProgress;
        PriceNextLvl = Lvls[LvlNext].Price;
        ReceptionistCount = Lvls[Lvl].ReceptionistCount;
    }
}

public sealed class UtilityModel : EntityModel
{
    public UtilityModel(string id, int lvl, EntityType type, UtilityConfig config) : base(id, lvl, type)
    {
        TargetPurchaseValue = config.TargetPurchaseProgress;
        Area = config.Area;
    }
    public override int GetLvlLength()
    {
        return 0;
    }

    public override void GetUpdatedValues()
    {
    }
}

public sealed class ElevatorModel : EntityModel
{
    public int NextHotel;
    public int TransferRate;

    public ElevatorModel(string id, int lvl, EntityType type, ElevatorConfig config, int nextHotel, int maxLvl) : base(id, lvl, type)
    {
        TargetPurchaseValue = maxLvl;
        Area = config.Area;
        PricePurchase = config.PricePurchase;
        NextHotel = nextHotel;
        TransferRate = config.TransferRate;
    }

    public override int GetLvlLength()
    {
        return 0;
    }

    public override void GetUpdatedValues()
    {
    }
}

public sealed class AreaModel : EntityModel
{
    public int TransferRate;
    public AreaModel(string id, int lvl, EntityType type, AreaConfig config) : base(id, lvl, type)
    {
        PricePurchase = config.PricePurchase;
        TargetPurchaseValue = config.TargetLvl;
        TransferRate = config.TransferRate;
        UpdateModel();
    }

    public override int GetLvlLength()
    {
        return 0;
    }

    public override void GetUpdatedValues()
    {
    }
}

public sealed class RoomModel : EntityModel
{
    public int VisualIndex;
    public float CleaningTime;
    public int StayFee;
    public float StayDuration;
    public int Limit;
    public int TransferRate;

    public RoomLvlConfig[] Lvls;

    public RoomModel(string id, int lvl, EntityType type, RoomConfig config, int visualIndex) : base(id, lvl, type)
    {
        VisualIndex = visualIndex;
        StayDuration = config.StayDuration;
        StayFee = config.StayFee;
        TargetPurchaseValue = config.TargetPurchaseProgress;
        PricePurchase = config.PricePurchase;
        PurchaseProgressReward = config.PurchaseProgressReward;
        UpdateProgressReward = config.UpdateProgressReward;
        Area = config.Area;
        Limit = config.Limit;
        TransferRate = config.TransferRate;
        Lvls = config.Lvls;

        UpdateModel();
    }

    public override void GetUpdatedValues()
    {
        TargetUpdateProgress = Lvls[LvlNext].TargetUpdateProgress;
        PriceNextLvl = Lvls[LvlNext].Price;
        CleaningTime = Lvls[Lvl].CleaningTime;
    }

    public override int GetLvlLength()
    {
        return Lvls.Length;
    }
}

public abstract class EntityController
{
    public int CameraAngleSign;
    public Transform Transform;

    public bool IsUpdatable(bool isMaxed, int currentProgress, int targetUpdateProgress)
    {
        return !isMaxed && currentProgress >= targetUpdateProgress;
    }

    public bool IsPurchasable(bool isAreaPurchased, int currentValue, int targetValue)
    {
        return isAreaPurchased && currentValue >= targetValue;
    }
}

public sealed class RoomController : EntityController, IDisposable
{
    private readonly RoomModel _model;
    private readonly RoomView _view;
    private readonly StateManager<RoomState> _stateManager;

    private readonly List<ItemRoomController> _items;

    private readonly ItemController _itemCashPile;
    private readonly ItemController _itemBuyUpdate;

    public RoomView View => _view;
    public RoomModel Model => _model;

    public List<ItemRoomController> Items => _items;

    public ItemController ItemCashPile => _itemCashPile;
    public ItemController ItemBuyUpdate => _itemBuyUpdate;

    public AreaController Area { get; internal set; }
    public bool IsAvailable { get; internal set; }

    public RoomController(RoomView view, Context context)
    {
        _view = view;

        CameraAngleSign = _view.CameraAngleSign;
        Transform = _view.transform;

        var subContext = new Context(context);
        var injector = new Injector(subContext);

        subContext.Install(this);
        subContext.Install(injector);

        _stateManager = new StateManager<RoomState>();
        injector.Inject(_stateManager);

        var gameManager = context.Get<GameManager>();
        var gameConfig = context.Get<GameConfig>();

        string id = gameManager.Model.GenerateEntityID(gameManager.Model.Hotel, _view.Type, view.Config.Number);
        _view.name = id;
        int lvl = gameManager.Model.LoadPlaceLvl(id);
        int visualIndex = gameManager.Model.LoadPlaceVisualIndex(id);

        _model = new RoomModel(id, lvl, _view.Type, _view.Config, visualIndex)
        {
            IsPurchased = gameManager.Model.LoadPlaceIsPurchased(id)
        };
        _model.Cash = gameManager.Model.LoadPlaceCash(_model.ID);

        _view.HudView.Model = _model;

        _items = new List<ItemRoomController>();
        foreach (var itemView in _view.Items)
        {
            var item = new ItemRoomController(itemView.transform, gameConfig.RoomItemRadius, itemView.Type, itemView, _view.Config.Area);
            _items.Add(item);
        }

        _itemCashPile = new ItemController(_view.ItemCashPileView.transform, gameConfig.CashPileRadius, _view.ItemCashPileView.Type);
        _itemBuyUpdate = new ItemController(_view.ItemBuyUpdateView.transform, gameConfig.BuyUpdateRadius, _view.ItemBuyUpdateView.Type);

        SwitchToState(new RoomInitializeState());
    }

    public void SwitchToState<T>(T instance) where T : RoomState
    {
        _stateManager.SwitchToState(instance);
    }

    public void Dispose()
    {
        _stateManager.Dispose();
    }

    public int GetTotalReward()
    {
        int lvlCount = _model.Lvls.Length - 1;
        int purchaseReward = _model.PurchaseProgressReward;
        int updateReward = _model.UpdateProgressReward;
        int reward = purchaseReward + (lvlCount * updateReward);
        return reward;
    }
}

