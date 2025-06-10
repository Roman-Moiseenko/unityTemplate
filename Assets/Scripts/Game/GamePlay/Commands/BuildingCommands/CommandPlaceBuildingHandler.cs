using System.Linq;
using Game.State.Mergeable.Buildings;
using Game.State.Root;
using MVVM.CMD;
using UnityEngine;

namespace Game.GamePlay.Commands.BuildingCommands
{
    public class CommandPlaceBuildingHandler : ICommandHandler<CommandPlaceBuilding>
    {
        private readonly GameplayStateProxy _gameplayState;

        public CommandPlaceBuildingHandler(GameplayStateProxy gameplayState)
        {
            _gameplayState = gameplayState;
        }
        
        public bool Handle(CommandPlaceBuilding command)
        {
            /*
            var currentMap = _gameplayState.Maps.FirstOrDefault(m => m.Id == _gameplayState.CurrentMapId.CurrentValue);
            if (currentMap == null)
            {
                Debug.Log($" Карта не найдена { _gameplayState.CurrentMapId.CurrentValue}");
                return false;
            }
            */
            var entityId = _gameplayState.CreateEntityID(); //Получаем уникальный ID
            
   //         var newBuildingEntity = new BuildingEntity //Создаем сущность игрового объекта
   //         { };
          //  var newBuildingEntityProxy = new BuildingEntityProxy(newBuildingEntity); //Оборачиваем его Прокси
           // currentMap.Entities.Add(newBuildingEntityProxy);//Добавляем в список объектов карты
            //_gameState.Buildings.Add(newBuildingEntityProxy); 
            
            return true;
        }
    }
}
