using Game.Config;
using UnityEngine;

public sealed class ToiletView : PlaceWithItemsAimView
{
    [SerializeField] private RouteView _line;
    [SerializeField] private ToiletConfig _config;

    public RouteView Line => _line;
    public ToiletConfig Config => _config;
}