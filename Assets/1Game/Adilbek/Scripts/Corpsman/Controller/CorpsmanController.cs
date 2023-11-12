using Game;
using Game.Config;
using Game.Core;
using Injection;
using System;
using UnityEngine;

public class CorpsmanController : EntityController, IDisposable
{
    private readonly CorpsmanModel _model;
    private readonly CorpsmanView _view;
    private readonly CorpsmanUnitView _unitView;
    private readonly StateManager<CorpsmanState> _stateManager;
    private readonly ItemController _itemBuyUpdate;

    public CorpsmanView View => _view;
    public CorpsmanUnitView UnitView => _unitView;
    public CorpsmanModel Model => _model;
    public ItemController ItemBuyUpdate => _itemBuyUpdate;

    public Vector3 InitialPosition { get; internal set; }

    public CorpsmanController(CorpsmanView view, CorpsmanUnitView unitView, Context context)
    {
        _view = view;
        _unitView = unitView;

        var subContext = new Context(context);
        var injector = new Injector(subContext);

        subContext.Install(this);
        subContext.InstallByType(this, typeof(CorpsmanController));
        subContext.Install(injector);

        _stateManager = new StateManager<CorpsmanState>();
        injector.Inject(_stateManager);

        var gameManager = context.Get<GameManager>();
        var gameConfig = context.Get<GameConfig>();
        string id = gameManager.Model.GenerateEntityID(gameManager.Model.Hotel, _view.Type, view.Config.Number);
        _view.name = id;
        int lvl = gameManager.Model.LoadPlaceLvl(id);

        _model = new CorpsmanModel(id, lvl, _view.Type, _view.Config)
        {
            IsPurchased = gameManager.Model.LoadPlaceIsPurchased(id)
        };
        _model.Cash = gameManager.Model.LoadPlaceCash(_model.ID);

        _view.HudView.Model = _model;

        _itemBuyUpdate = new ItemController(view.ItemBuyUpdateView.transform, gameConfig.BuyUpdateRadius, view.ItemBuyUpdateView.Type);

        SwitchToState(new CorpsmanInitializeState());
    }

    public void SwitchToState<T>(T instance) where T : CorpsmanState
    {
        _stateManager.SwitchToState(instance);
    }

    public void Dispose()
    {
        _stateManager.Dispose();
    }
}
