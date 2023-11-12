using Game;
using Game.Core;
using Game.UI;
using Injection;
using System.Linq;

public abstract class CorpsmanState : State
{

    [Inject] protected CorpsmanController _corpsman;
    [Inject] protected Timer _timer;
    [Inject] protected GameManager _gameManager;
    [Inject] protected GameView _gameView;

    public override void Dispose()
    {
    }

    public override void Initialize()
    {
    }

    internal void FindUsedItem()
    {
        var toilet = _gameManager.FindToilet(_corpsman.Model.Area);
        if (toilet != null)
        {
            ItemController cabineResult = toilet.GetUnavailableCabine();
            if (cabineResult != null)
            {
                UtilityController utility = _gameManager.FindUnility();
                if (utility != null)
                {
                    if (_corpsman.Model.Item == null)
                    {
                        ItemController itemController = utility.Items.First();
                        if (itemController != null)
                        {
                            _corpsman.SwitchToState(new CorpsmanWalkToToiletStock(itemController));
                        }
                    }
                    else
                    {
                        _corpsman.SwitchToState(new CorpsmanWalkToToilet(cabineResult));
                    }

                }
            }

        }

    }
}
