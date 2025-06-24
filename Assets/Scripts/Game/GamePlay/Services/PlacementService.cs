using System.Collections.Generic;
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

        public PlacementService(GameplayStateProxy gameplayState)
        {
            _gameplayState = gameplayState;
        }


        public bool CheckPlacementTower(Vector2Int position, int TowerId)
        {
            
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
            foreach (var entityData in _gameplayState.Entities)
            {
                //На другой башне
                if (entityData is TowerEntity towerEntity && towerEntity.UniqueId != TowerId)
                {
                    if (towerEntity.PositionNear(position)) result = true;
                    
                    if (position == towerEntity.Position.CurrentValue) return false;  //Строить нельзя, принудительный выход
                }
            }

            foreach (var roadEntity in _gameplayState.Way)
            {
                if (position == roadEntity.Position.CurrentValue) return false; //На дороге
                if (roadEntity.PositionNear(position)) result = true;
            }
            

            return result;
        }

        public Vector2Int GetNewPositionTower()
        {
            foreach (var roadEntity in _gameplayState.Way)
            {
                foreach (var nearPosition in roadEntity.GetNearPositions())
                {
                    if (CheckPlacementTower(nearPosition, -1)) return nearPosition;
                }
            }
            
            return _gameplayState.Way[0].Position.CurrentValue; //Вычисляем координаты для башни 
        }

        public Vector2Int GetNewPositionRoad()
        {
            return new Vector2Int(Random.Range(-1, 5), Random.Range(-1, 3));

        }

        public bool CheckPlacementRoad(Vector2Int position, List<RoadEntityData> roads)
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
            foreach (var groundEntity in _gameplayState.Entities)
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
            foreach (var VARIABLE in _gameplayState.Entities)
            {
                
                
            }
            
            
            //TODO проверить все дороги, попадает хотя бы одна на землю, и не попадает ли каждая на крепость, башню и дорогу
            //TODO Проверить крайние на совпадение с маршрутом Way или WaySecond
            return result;
        }

        public Vector2Int GetNewPositionGround()
        {
            return new Vector2Int(Random.Range(-1, 5), Random.Range(-1, 3));

        }

        private bool IsCastle(Vector2Int position)
        {
            if ((position.y == -1 || position.y == 0 || position.y == 1) && (position.x == -1 || position.x == 0))
                return true;
            return false;

        }
    }
}