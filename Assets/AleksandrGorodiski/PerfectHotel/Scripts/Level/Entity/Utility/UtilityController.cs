using System;
using System.Collections.Generic;
using Game;
using Game.Config;
using Game.Core;
using Game.Level.Utility.UtilityState;
using Injection;

public sealed class UtilityController : EntityController, IDisposable
{
    private readonly UtilityModel _model;
    private readonly UtilityView _view;
    private readonly StateManager<UtilityState> _stateManager;
    private readonly List<ItemController> _items;

    public EntityModel Model => _model;
    public UtilityView View => _view;
    public List<ItemController> Items => _items;

    public UtilityController(UtilityView view, Context context)
    {
        _view = view;

        CameraAngleSign = _view.Sign;
        Transform = _view.transform;

        var subContext = new Context(context);
        var injector = new Injector(subContext);

        subContext.Install(this);
        subContext.InstallByType(this, typeof(UtilityController));
        subContext.Install(injector);

        _stateManager = new StateManager<UtilityState>();
        injector.Inject(_stateManager);

        var gameManager = context.Get<GameManager>();
        var gameConfig = context.Get<GameConfig>();

        string id = gameManager.Model.GenerateEntityID(gameManager.Model.Hotel, _view.Type, 0);
        _view.name = id;

        _model = new UtilityModel(id, 0, _view.Type, _view.Config);

        _items = new List<ItemController>();
        foreach (var itemView in _view.Items)
        {
            var inventory = new ItemUtilityController(itemView.transform, gameConfig.UtilityInventoryRadius, itemView.Type, itemView);
            _items.Add(inventory);
        }

        SwitchToState(new UtilityInitializeState());
    }

    public void SwitchToState<T>(T instance) where T : UtilityState
    {
        _stateManager.SwitchToState(instance);
    }

    public void Dispose()
    {
        _stateManager.Dispose();
    }
}
