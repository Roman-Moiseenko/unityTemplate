using System.Linq;
using Game.State.CMD;
using Game.State.Mergeable.Buildings;
using Game.State.Root;
using UnityEngine;

namespace Game.GamePlay.Commands.BuildingCommands
{
    public class CommandPlaceBuildingHandler : ICommandHandler<CommandPlaceBuilding>
    {
        private readonly GameStateProxy _gameState;

        public CommandPlaceBuildingHandler(GameStateProxy gameState)
        {
            _gameState = gameState;
        }
        
        public bool Handle(CommandPlaceBuilding command)
        {
            var currentMap = _gameState.Maps.FirstOrDefault(m => m.Id == _gameState.CurrentMapId.CurrentValue);
            if (currentMap == null)
            {
                Debug.Log($" Карта не найдена { _gameState.CurrentMapId.CurrentValue}");
                return false;
            }
            
            var entityId = _gameState.CreateEntityID(); //Получаем уникальный ID
   //         var newBuildingEntity = new BuildingEntity //Создаем сущность игрового объекта
   //         { };
          //  var newBuildingEntityProxy = new BuildingEntityProxy(newBuildingEntity); //Оборачиваем его Прокси
           // currentMap.Entities.Add(newBuildingEntityProxy);//Добавляем в список объектов карты
            //_gameState.Buildings.Add(newBuildingEntityProxy); 
            
            return true;
        }
    }
}
