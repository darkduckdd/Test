using System;
using Game.Config;
using Game.Core;
using Game.Level.Unit;
using Game.Level.Unit.UnitState;
using Injection;

public sealed class UnitController
{
    public Action<UnitController> ON_REMOVE;
    public Action<UnitController> ON_REMOVE_FROM_LINE;

    private readonly StateManager<UnitState> _stateManager;

    private UnitView _view;
    private UnitModel _model;

    public UnitView View => _view;
    public UnitModel Model => _model;

    public int Area;

    public UnitController(UnitView view, int index, Context context)
    {
        _view = view;

        _view.Index = index;

        var subContext = new Context(context);
        var injector = new Injector(subContext);

        subContext.Install(this);
        subContext.Install(injector);

        _stateManager = new StateManager<UnitState>();
        injector.Inject(_stateManager);

        var config = context.Get<GameConfig>();
        _model = new UnitModel(config.CustomerWalkSpeed, config.PlayerRotationSpeed);

        _view.NavMeshAgent.speed = _model.WalkSpeed;
    }

    public void SwitchToState<T>(T instance) where T : UnitState
    {
        _stateManager.SwitchToState(instance);
    }

    public void Dispose()
    {
        _stateManager.Dispose();
    }

    public void FireUnitRemove()
    {
        ON_REMOVE?.Invoke(this);
    }

    public void FireUnitRemoveFromLine()
    {
        ON_REMOVE_FROM_LINE?.Invoke(this);
    }
}

