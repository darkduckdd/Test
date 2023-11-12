using Game.Level;
using Game.Level.Cash;
using Injection;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Modules.CashModule
{
    public sealed class CashModule : Module<CashModuleView>
    {
        private const float _heightAbovePile = 3f;
        private const float _heightAbovePlayer = 1.5f;
        private const float _cashFlyToRemoveRate = 0.1f;

        [Inject] private GameManager _gameManager;
        [Inject] private Context _context;

        private readonly Dictionary<CashPileView, CashPileController> _cashPilesMap;
        private readonly Dictionary<ItemController, CashPileView> _itemsMap;
        private List<CashController> _tempCashes;

        private float _cashFlyToRemoveTimer;

        public CashModule(CashModuleView view) : base(view)
        {
            _cashPilesMap = new Dictionary<CashPileView, CashPileController>();
            _itemsMap = new Dictionary<ItemController, CashPileView>();
            _tempCashes = new List<CashController>();
        }

        public override void Initialize()
        {
            AddCashPile(_gameManager.Reception.View.CashPileView, _gameManager.Reception.ItemCashPile, _gameManager.Reception.Model);

            foreach (var room in _gameManager.Rooms)
            {
                AddCashPile(room.View.CashPileView, room.ItemCashPile, room.Model);
            }

            foreach (var toilet in _gameManager.Toilets)
            {
                AddCashPile(toilet.View.CashPileView, toilet.ItemCashPile, toilet.Model);
            }

            _gameManager.FLY_TO_REMOVE_CASH += CashFlyToRemove;
        }

        private void AddCashPile(CashPileView view, ItemController itemCashPile, EntityModel model)
        {
            view.CASH_FLY_TO_PILE += CashFlyToPile;
            view.CASH_FLY_TO_PLAYER += CashFlyToPlayer;

            var pile = new CashPileController(view, model);

            _cashPilesMap[view] = pile;
            _itemsMap[itemCashPile] = view;

            itemCashPile.PLAYER_ON_ITEM += PlayerOnItem;

            _gameManager.AddItem(itemCashPile);
        }

        public override void Dispose()
        {
            _gameManager.FLY_TO_REMOVE_CASH -= CashFlyToRemove;

            foreach (var cashPile in _cashPilesMap.Values)
            {
                cashPile.View.CASH_FLY_TO_PILE -= CashFlyToPile;
                cashPile.View.CASH_FLY_TO_PLAYER -= CashFlyToPlayer;

                foreach (var cash in cashPile.View.Cashes)
                {
                    cash.REMOVE_CASH -= OnRemoveCash;
                    cash.Dispose();
                }
                cashPile.View.Cashes.Clear();
            }

            foreach (var cash in _tempCashes)
            {
                cash.REMOVE_CASH -= OnRemoveCash;
                cash.Dispose();
            }
            _tempCashes.Clear();

            _view.ReleaseAllInstances();

            foreach (var item in _itemsMap.Keys)
            {
                item.PLAYER_ON_ITEM -= PlayerOnItem;
            }
        }

        private CashController Cash(Vector3 position)
        {
            var cashView = _view.Get<CashView>();
            var cash = new CashController(cashView, position, _context);
            return cash;
        }

        private void CashFlyToPile(CashPileView cashPileView, Vector3 endPosition)
        {
            CashController cash = Cash(cashPileView.transform.position + (Vector3.up * _heightAbovePile));
            cash.FlyToPile(endPosition);
            cashPileView.Cashes.Add(cash);
            cash.REMOVE_CASH += OnRemoveCash;
        }

        private void CashFlyToPlayer(CashPileView cashPileView, int index)
        {
            /* var cash = cashPileView.Cashes[index];
             cash.FlyToPlayer();
             cashPileView.Cashes.Remove(cash);
             _tempCashes.Add(cash);*/
            foreach (var cash in cashPileView.Cashes)
            {
                _tempCashes.Add(cash);
                cash.FireRemoveCash();
            }
            cashPileView.Cashes.Clear();

        }

        private void CashFlyToRemove(Vector3 endPosition)
        {
            _cashFlyToRemoveTimer += Time.deltaTime;
            if (_cashFlyToRemoveTimer < _cashFlyToRemoveRate) return;

            _cashFlyToRemoveTimer = 0f;

            CashController cash = Cash(_gameManager.Player.View.transform.position + (Vector3.up * _heightAbovePlayer));
            cash.FlyToRemove(endPosition);
            cash.REMOVE_CASH += OnRemoveCash;
        }

        private void OnRemoveCash(CashController cash)
        {
            cash.REMOVE_CASH -= OnRemoveCash;
            _view.Release(cash.View);
            cash.Dispose();
            _tempCashes.Remove(cash);
        }

        private void PlayerOnItem(ItemController item)
        {
            var cashPileView = _itemsMap[item];
            var cashPile = _cashPilesMap[cashPileView];

            if (cashPile.Model.Cash <= 0) return;

            int amount = 1;
            cashPile.Model.Cash -= amount;
            _gameManager.Model.SavePlaceCash(cashPile.Model.ID, cashPile.Model.Cash);
            cashPile.Model.SetChanged();

            _gameManager.Model.Cash += amount;
            _gameManager.Model.Save();
            _gameManager.Model.SetChanged();
        }
    }
}