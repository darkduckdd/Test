using Core;
using Game.Config;
using Game.Core;
using Game.Level.Player.PlayerState;
using Injection;
using UnityEngine;

public sealed class UnitModel : Observable
{
    public float WalkSpeed;
    public float RotateSpeed;

    public UnitModel(float walkSpeed, float rotateSpeed)
    {
        WalkSpeed = walkSpeed;
        RotateSpeed = rotateSpeed;
    }
}

public sealed class PlayerController
{
    public readonly UnitModel Model;
    private readonly PlayerView _view;
    private readonly StateManager<PlayerState> _stateManager;

    public Transform Transform => _view.transform;
    public PlayerView View => _view;

    public PlayerController(PlayerView view, Context context)
    {
        _view = view;
        var subContext = new Context(context);
        var injector = new Injector(subContext);
        subContext.Install(this);
        subContext.InstallByType(this, typeof(PlayerController));
        subContext.Install(injector);
        _stateManager = new StateManager<PlayerState>();
        injector.Inject(_stateManager);

        var config = context.Get<GameConfig>();
        Model = new UnitModel(config.PlayerWalkSpeed, config.PlayerRotationSpeed);
    }

    public void Dispose()
    {
        _stateManager.Dispose();
        GameObject.Destroy(_view.gameObject);
    }

    public void SwitchToState<T>(T instance) where T : PlayerState
    {
        _stateManager.SwitchToState(instance);
        _view.CurrentState = instance.GetType();
    }

    public Vector3 GetInventoryPosition(int index)
    {
        return _view.InventoryPosition + new Vector3(0f, index * _view.InventoryHeight, 0f);
    }
}

