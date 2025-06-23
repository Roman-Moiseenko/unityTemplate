using System.Collections.Generic;
using Game.GamePlay.View.Roads;
using Game.State.Maps.Grounds;
using Game.State.Maps.Towers;
using Game.State.Root;
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
            if ((position.y == -1 || position.y == 0 || position.y == 1) && (position.x == -1 || position.x == 0))
            {
                return false; //Строить нельзя, принудительный выход
            }
            
            //Сначала проверяем землю
            foreach (var entityData in _gameplayState.Entities)
            {
                //Проверяем на земле или нет
                if (entityData is GroundEntity groundEntity)
                {
                    if (position == groundEntity.Position.CurrentValue) result = true;
                }
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
                
                //TODO Проверка на размещение возле другой башни
                
                //Возле дороги или другой башни
            }

            foreach (var roadEntity in _gameplayState.Way)
            {
                if (roadEntity.PositionNear(position)) result = true;
                
                if (position == roadEntity.Position.CurrentValue) return false;
                
            }
            //На дороге
            //if (position == new Vector2Int(0, 0)) result = false;

            return result;
        }

        public Vector2Int GetNewPositionTower()
        {
            //TODO Вычисляем координаты для башни 
            return new Vector2Int(Random.Range(-1, 5), Random.Range(-1, 3));
        }

        public Vector2Int GetNewPositionRoad()
        {
            return new Vector2Int(Random.Range(-1, 5), Random.Range(-1, 3));

        }

        public bool CheckPlacementRoad(Vector2Int position, List<RoadViewModel> getRoadIds)
        {
            //TODO проверить все дороги, попадает хотя бы одна на землю, и не попадает ли каждая на крепость, башню и дорогу
            //TODO Проверить крайние на совпадение с маршрутом Way или WaySecond
            return true;
        }
    }
}