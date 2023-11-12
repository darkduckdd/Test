using System;
using Game;
using Game.Config;
using Game.Core;
using Game.Level.Cleaner.CleanerState;
using Injection;
using UnityEngine;

public sealed class CleanerController : EntityController, IDisposable
{
    private readonly CleanerModel _model;
    private readonly CleanerView _view;
    private readonly CleanerUnitView _unitView;
    private readonly StateManager<CleanerState> _stateManager;
    private readonly ItemController _itemBuyUpdate;

    public CleanerView View => _view;
    public CleanerUnitView UnitView => _unitView;
    public CleanerModel Model => _model;
    public ItemController ItemBuyUpdate => _itemBuyUpdate;

    public Vector3 InitialPosition { get; internal set; }

    public CleanerController(CleanerView view, CleanerUnitView unitView, Context context)
    {
        _view = view;
        _unitView = unitView;

        var subContext = new Context(context);
        var injector = new Injector(subContext);

        subContext.Install(this);
        subContext.InstallByType(this, typeof(CleanerController));
        subContext.Install(injector);

        _stateManager = new StateManager<CleanerState>();
        injector.Inject(_stateManager);

        var gameManager = context.Get<GameManager>();
        var gameConfig = context.Get<GameConfig>();
        string id = gameManager.Model.GenerateEntityID(gameManager.Model.Hotel, _view.Type, view.Config.Number);
        _view.name = id;
        int lvl = gameManager.Model.LoadPlaceLvl(id);

        _model = new CleanerModel(id, lvl, _view.Type, _view.Config);
        _model.IsPurchased = gameManager.Model.LoadPlaceIsPurchased(id);
        _model.Cash = gameManager.Model.LoadPlaceCash(_model.ID);

        _view.HudView.Model = _model;

        _itemBuyUpdate = new ItemController(view.ItemBuyUpdateView.transform, gameConfig.BuyUpdateRadius, view.ItemBuyUpdateView.Type);

        SwitchToState(new CleanerInitializeState());
    }

    public void SwitchToState<T>(T instance) where T : CleanerState
    {
        _stateManager.SwitchToState(instance);
    }

    public void Dispose()
    {
        _stateManager.Dispose();
    }
}
