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
            
            foreach (var entityData in _gameplayState.Entities)
            {
                //Проверяем на земле или нет
                if (entityData is GroundEntity groundEntity)
                {
                    if (position == groundEntity.Position.CurrentValue) result = true;
                }
                //На другой башне
                if (entityData is TowerEntity towerEntity && towerEntity.UniqueId != TowerId)
                {
                    if (position == towerEntity.Position.CurrentValue) result = false;
                }
                //На дороге
                
                //Возле дороги или другой башни
                
            }
            //На замке
            if (position == new Vector2Int(0, 0)) result = false;

            return result;
        }
    }
}