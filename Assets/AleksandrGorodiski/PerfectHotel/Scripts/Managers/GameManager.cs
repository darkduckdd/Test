using Game.Config;
using Game.Domain;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public sealed class GameManager : IDisposable
    {
        public event Action<Vector3, int> ADD_GAME_PROGRESS;
        public event Action<int> PROGRESS_CHANGED;
        public event Action<int> LEVEL_CHANGED;
        public event Action<AreaController> AREA_PURCHASED;
        public event Action<Vector3> FLY_TO_REMOVE_CASH;
        public event Action ITEM_ADDED;
        public event Action<Vector3, int> ON_NOTIFICATION_NEED_LVL;
        public event Action<InventoryType, Vector3, Vector3, float> FLY_INVENTORY_ITEM;
        public event Action ELEVATOR_PURCHASED;

        private List<ItemController> _items;

        public readonly GameModel Model;
        public PlayerController Player;
        public ReceptionController Reception;
        public List<RoomController> Rooms;
        public List<ToiletController> Toilets;
        public List<AreaController> Areas;
        public ElevatorController Elevator;
        public List<EntityController> Entities;
        public readonly Dictionary<UnitController, RoomController> CustomerRoomMap;
        public List<UtilityController> Utilities;

        public GameManager(GameConfig config)
        {
            Model = GameModel.Load(config);
            Areas = new List<AreaController>();
            Rooms = new List<RoomController>();
            CustomerRoomMap = new Dictionary<UnitController, RoomController>();
            _items = new List<ItemController>();
            Toilets = new List<ToiletController>();
            Entities = new List<EntityController>();
            Utilities = new List<UtilityController>();
        }

        public void Dispose()
        {
        }

        public void AddItem(ItemController item)
        {
            if (!_items.Contains(item))
            {
                _items.Add(item);
                ITEM_ADDED.SafeInvoke();
            }
        }

        public void RemoveItem(ItemController item)
        {
            if (_items.Contains(item))
            {
                _items.Remove(item);
            }
        }

        internal void FireElevatorPurchased()
        {
            ELEVATOR_PURCHASED?.Invoke();
        }

        public RoomController FindAvailableRoom()
        {
            foreach (var room in Rooms)
            {
                if (room != null && room.IsAvailable == true)
                    return room;
            }
            return null;
        }

        public ItemController FindUsedItemForCleaner(int area)
        {
            foreach (var item in _items)
            {
                ItemRoomController itemRoom = item as ItemRoomController;

                if (itemRoom != null && itemRoom.Area == area && itemRoom.Type == ItemType.Clean)
                    return item;
            }
            return null;
        }

        public ItemController FindClosestUsedItem()
        {
            foreach (var item in _items)
            {
                if (item != null && Vector3.Distance(item.Transform.position, Player.View.transform.position) < item.Radius)
                    return item;
            }
            return null;
        }

        public EntityController FindClosestEntity(float radius)
        {
            foreach (var entity in Entities)
            {
                if (entity != null && Vector3.Distance(entity.Transform.position, Player.View.transform.position) < radius)
                    return entity;
            }
            return null;
        }

        public ToiletController FindToilet(int area)
        {
            foreach (var toilet in Toilets)
            {
                if (toilet != null && toilet.Model.IsPurchased && toilet.Model.Area == area)
                    return toilet;
            }
            return null;
        }

        internal AreaController FindArea(int number)
        {
            foreach (var area in Areas)
            {
                if (area.Number == number)
                    return area;
            }
            Log.Error("Error. Area " + number + " not found");
            return null;
        }

        public UtilityController FindUnility()
        {
            foreach (var utility in Utilities)
            {
                return utility;
            }
            return null;
        }

        public void FireAddGameProgress(Vector3 position, int progressDelta)
        {
            ADD_GAME_PROGRESS.SafeInvoke(position, progressDelta);
        }

        public void FireProgressChanged(int progress)
        {
            PROGRESS_CHANGED.SafeInvoke(progress);
        }

        public void FireAreaPurchased(AreaController area)
        {
            AREA_PURCHASED.SafeInvoke(area);
        }

        public void FireLevelChanged(int lvl)
        {
            LEVEL_CHANGED.SafeInvoke(lvl);
        }

        public void FireFlyToRemoveCash(Vector3 endPosition)
        {
            FLY_TO_REMOVE_CASH.SafeInvoke(endPosition);
        }

        internal void FireFlyInventoryItem(InventoryType type, Vector3 startPosition, Vector3 endPosition, float flyTime)
        {
            FLY_INVENTORY_ITEM.SafeInvoke(type, startPosition, endPosition, flyTime);
        }

        internal void FireNotificationNeedLvl(Vector3 itemPosition, int lvl)
        {
            ON_NOTIFICATION_NEED_LVL.SafeInvoke(itemPosition, lvl);
        }
    }
}