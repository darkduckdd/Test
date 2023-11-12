using System;
using Game;
using Game.Config;
using Game.Core;
using Game.Level.Area.AreaState;
using Injection;

public sealed class AreaController : IDisposable
{
    private readonly AreaModel _model;
    private readonly AreaView _view;
    private readonly StateManager<AreaState> _stateManager;

    private readonly ItemController _itemBuyUpdate;

    public AreaModel Model => _model;
    public AreaView View => _view;

    public ItemController ItemBuyUpdate => _itemBuyUpdate;

    private int _number;
    public int Number => _number;

    public AreaController(AreaView view, Context context)
    {
        _view = view;

        var subContext = new Context(context);
        var injector = new Injector(subContext);

        subContext.Install(this);
        subContext.Install(injector);

        _stateManager = new StateManager<AreaState>();
        injector.Inject(_stateManager);

        var gameManager = context.Get<GameManager>();
        var gameConfig = context.Get<GameConfig>();

        _number = _view.Config.Number;
        string id = gameManager.Model.GenerateEntityID(gameManager.Model.Hotel, _view.Type, _number);
        _view.name = id;
        int lvl = gameManager.Model.LoadPlaceLvl(id);

        _model = new AreaModel(id, lvl, _view.Type, _view.Config);
        _model.IsPurchased = gameManager.Model.LoadPlaceIsPurchased(id);

        _view.HudView.Model = _model;

        _itemBuyUpdate = new ItemController(_view.ItemBuyUpdateView.transform, gameConfig.BuyUpdateRadius, _view.ItemBuyUpdateView.Type);
    }

    public void SwitchToState<T>(T instance) where T : AreaState
    {
        _stateManager.SwitchToState(instance);
    }

    public void Dispose()
    {
        _stateManager.Dispose();
    }

    public bool IsPurchasable(int lvl, int targetLvl)
    {
        return lvl >= targetLvl;
    }
}
