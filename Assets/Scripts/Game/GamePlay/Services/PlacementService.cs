using System;
using System.Collections.Generic;
using System.Linq;
using Game.GamePlay.View.Roads;
using Game.State.Maps.Grounds;
using Game.State.Maps.Roads;
using Game.State.Maps.Towers;
using Game.State.Root;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.GamePlay.Services
{
    /**
     * Сервис проверки размещения объекта на карте
     */
    public class PlacementService
    {
        private readonly GameplayStateProxy _gameplayState;
        private readonly WayService _wayService;

        public PlacementService(GameplayStateProxy gameplayState, WayService wayService)
        {
            _gameplayState = gameplayState;
            _wayService = wayService;
        }

        public Vector2Int GetDirectionTower(Vector2Int positionTower)
        {
            foreach (var roadEntity in _gameplayState.Way)
            {
                if (IsPositionNear(positionTower, roadEntity.Position.CurrentValue))
                    return roadEntity.Position.CurrentValue - positionTower;
            }

            foreach (var roadEntity in _gameplayState.WaySecond)
            {
                if (IsPositionNear(positionTower, roadEntity.Position.CurrentValue))
                    return roadEntity.Position.CurrentValue - positionTower;
            }

            return Vector2Int.zero;
        }

        /**
         * Определяем возможность размещения Башни на карте
         */
        public bool CheckPlacementTower(Vector2Int position, int towerId, bool onRoad, bool isPlacement)
        {
            //var tower = _gameplayState.Origin.Towers.Find(t => t.UniqueId == towerId);
            var result = false;

            //На замке
            if (IsCastle(position)) return false; //Строить нельзя, принудительный выход
            //Сначала проверяем землю
            foreach (var groundData in _gameplayState.Grounds)
            {
                //Проверяем на земле или нет
                if (position == groundData.Position.CurrentValue) result = true;
            }

            if (result == false) return false; //Не нашли участок для строительства, выходим

            result = false;

            //Проверяем башни
            foreach (var towerEntity in _gameplayState.Towers)
            {
                if (towerEntity.UniqueId != towerId) //На другой башне ?
                {
                    if (towerEntity.PositionNear(position)) result = true;
                    //Строить на башне нельзя, принудительный выход
                    if (position == towerEntity.Position.CurrentValue) return false;
                }
            }

            //Для Placement
            if (isPlacement)
            {
                var nearRoad = false;
                for (var x = position.x - 2; x <= position.x + 2; x++)
                {
                    for (var y = position.y - 2; y <= position.y + 2; y++)
                    {
                        if (IsRoad(new Vector2Int(x, y)))
                            nearRoad = true;
                    }
                }
                //Поблизости не оказалось дороги
                if (!nearRoad) return false;
                
            }
            
            //TODO протестировать
            foreach (var roadEntity in _gameplayState.Way)
            {
                if (onRoad)
                {
                    if (position == roadEntity.Position.CurrentValue) result = true;
                    if (roadEntity.PositionNear(position)) result = false;
                }
                else
                {
                    if (position == roadEntity.Position.CurrentValue) return false; //На дороге
                    if (roadEntity.PositionNear(position)) result = true;
                }
            }

            foreach (var roadEntity in _gameplayState.WaySecond)
            {
                if (onRoad)
                {
                    if (position == roadEntity.Position.CurrentValue) result = true;
                    if (roadEntity.PositionNear(position)) return false;
                }
                else
                {
                    if (position == roadEntity.Position.CurrentValue) return false; //На дороге
                    if (roadEntity.PositionNear(position)) result = true;
                }
            }
            //TODO Проверяем второй путь

            return result;
        }

        public bool IsRoad(Vector2Int position)
        {
            foreach (var roadEntity in _gameplayState.Way)
            {
                if (roadEntity.Position.CurrentValue == position) return true;
            }

            foreach (var roadEntity in _gameplayState.WaySecond)
            {
                if (roadEntity.Position.CurrentValue == position) return true;
            }

            return false;
        }

        public Vector2Int GetNewPositionTower(bool onRoad)
        {
            if (!onRoad) //Проверяем свободные места возле дороги
            {
                foreach (var roadEntity in _gameplayState.Way)
                {
                    foreach (var nearPosition in roadEntity.GetNearPositions())
                    {
                        if (CheckPlacementTower(nearPosition, -1, false, false)) return nearPosition;
                    }
                }
            }
            else //Проверяем свободные места на дороге, с обратного конца
            {
                for (var i = _gameplayState.Way.Count - 1; i >= 0; i--)
                {
                    var roadEntity = _gameplayState.Way[i];
                    var position = roadEntity.Position.CurrentValue;
                    if (CheckPlacementTower(position, -1, true, false)) return position;
                }
            }

            return _gameplayState.Way[0].Position.CurrentValue; //Вычисляем координаты для башни 
        }

        public Vector2Int GetNewPositionRoad()
        {
            return _wayService.GetExitPoint(_gameplayState.Origin.Way);
        }

        public Vector2Int GetNewDirectionRoad()
        {
            return _wayService.GetLastPoint(_gameplayState.Origin.Way);
        }

        public bool CheckPlacementRoad(List<RoadEntityData> roads)
        {
            var result = false;

            //На земле
            foreach (var groundEntity in _gameplayState.Grounds)
            {
                foreach (var road in roads)
                {
                    if (road.Position == groundEntity.Position.CurrentValue)
                    {
                        result = true;
                        break;
                    }

                    if (result) break;
                }
            }

            //На замке
            foreach (var road in roads)
            {
                if (IsCastle(road.Position)) return false; //Строить нельзя, принудительный выход 
            }

            //На башне
            foreach (var groundEntity in _gameplayState.Towers)
            {
                foreach (var road in roads)
                {
                    if (road.Position == groundEntity.Position.CurrentValue)
                    {
                        result = true;
                        break;
                    }

                    if (result) break;
                }
            }

            //На дороге 
            if (_gameplayState.Way.Any(roadWay => roads.Any(road => road.Position == roadWay.Position.CurrentValue)))
                return false;
            if (_gameplayState.WaySecond.Any(roadWay =>
                    roads.Any(road => road.Position == roadWay.Position.CurrentValue))) return false;
            if (_gameplayState.WayDisabled.Any(roadWay =>
                    roads.Any(road => road.Position == roadWay.Position.CurrentValue))) return false;
            var checkWay = CheckCombinationPointsRoad(_gameplayState.Origin.Way, roads);
            var checkWaySecond = CheckCombinationPointsRoad(_gameplayState.Origin.WaySecond, roads);

            result = checkWay ^ checkWaySecond; //Исключающее "или", Закольцовывание дороги

            return result;
        }

        public Vector2Int GetNewPositionGround()
        {
            return _wayService.GetExitPoint(_gameplayState.Origin.Way);
        }

        private bool IsCastle(Vector2Int position)
        {
            if ((position.y == -1 || position.y == 0 || position.y == 1) && (position.x == -1 || position.x == 0))
                return true;
            return false;
        }


        private bool CheckCombinationPointsRoad(List<RoadEntityData> way, List<RoadEntityData> roads)
        {
            if (way.Count == 0) return false;
            if (CheckForFirstPoint(way, roads)) return true;
            return CheckForLastPoint(way, roads);
        }

        private bool CheckForFirstPoint(List<RoadEntityData> way, List<RoadEntityData> roads)
        {
            if (way.Count == 0) return false;
            return _wayService.GetExitPoint(way) == _wayService.GetFirstPoint(roads)
                   && _wayService.GetLastPoint(way) == _wayService.GetEnterPoint(roads);
        }

        private bool CheckForLastPoint(List<RoadEntityData> way, List<RoadEntityData> roads, bool _t = false)
        {
            if (way.Count == 0) return false;
            return _wayService.GetExitPoint(way) == _wayService.GetLastPoint(roads)
                   && _wayService.GetLastPoint(way) == _wayService.GetExitPoint(roads);
        }

        /**
         * Присоединились к концу дороги
         */
        public bool IsLastPontForWay(List<RoadEntityData> roads)
        {
            return CheckForLastPoint(_gameplayState.Origin.Way, roads) ||
                   CheckForLastPoint(_gameplayState.Origin.WaySecond, roads);
        }

        /**
         * Присоединились к главной дороге
         */
        public bool IsMainWay(List<RoadEntityData> roads)
        {
            return CheckCombinationPointsRoad(_gameplayState.Origin.Way, roads);
        }

        public bool CheckPlacementFrameGround(Vector2Int position)
        {
            return _gameplayState.Origin.Grounds.Any(ground => ground.Position == position);
        }

        public bool IsPositionNear(Vector2Int target, Vector2Int position)
        {
            if (position.x == target.x && position.y == target.y - 1) return true;
            if (position.x == target.x && position.y == target.y + 1) return true;
            if (position.x == target.x - 1 && position.y == target.y) return true;
            if (position.x == target.x + 1 && position.y == target.y) return true;
            return false;
        }

        public bool IsWay(Vector2Int placementCurrentValue)
        {
            foreach (var roadEntity in _gameplayState.Way)
            {
                if (roadEntity.Position.CurrentValue == placementCurrentValue)
                    return true;
            }

            foreach (var roadEntity in _gameplayState.WaySecond)
            {
                if (roadEntity.Position.CurrentValue == placementCurrentValue)
                    return false;
            }

            throw new Exception("Неверные данные");
        }

        public Vector2Int GetDefaultPlacement(Vector2Int position)
        {
            List<Vector2Int> list = new();
            
            for (var x = position.x - 2; x <= position.x + 2; x++)
            {
                for (var y = position.y - 2; y <= position.y + 2; y++)
                {
                    if (IsRoad(new Vector2Int(x, y))) list.Add(new Vector2Int(x, y));
                }
            }

            var positionPlacement = Vector2Int.zero;
            var distance = 99f;
            foreach (var item in list)
            {
                //Сортировка по минимальному
                var d = Vector2Int.Distance(position, item);
                if (d < distance)
                {
                    distance = d;
                    positionPlacement = item;
                }
            }

            return positionPlacement;
        }
    }
}