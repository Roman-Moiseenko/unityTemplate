using System.Linq;
using Game.GamePlay.Commands.TowerCommand;
using Game.State.Maps.Towers;
using Game.State.Root;
using MVVM.CMD;
using UnityEngine;

namespace Game.GamePlay.Commands
{
    public class CommandPlaceTowerHandler : ICommandHandler<CommandPlaceTower>
    {
        private readonly GameplayStateProxy _gameplayState;

        public CommandPlaceTowerHandler(GameplayStateProxy gameplayState)
        {
            _gameplayState = gameplayState;
        }
        
        public bool Handle(CommandPlaceTower command)
        {
            var Entities = _gameplayState.Entities;
            /*
            var currentMap = _gameState.Maps.FirstOrDefault(m => m.Id == _gameState.CurrentMapId.CurrentValue);
            if (currentMap == null)
            {
                Debug.Log($" Карта не найдена { _gameState.CurrentMapId.CurrentValue}");
                return false;
            }
            */
           // var entityId = _gameState.CreateEntityID(); //Получаем уникальный ID
            var newTowerEntity = new TowerEntityData() //Создаем сущность игрового объекта
            {
                Position = command.Position,
                ConfigId = command.TowerTypeId,
            };
            var newTower = new TowerEntity(newTowerEntity); //Оборачиваем его Прокси
            Entities.Add(newTower);//Добавляем в список объектов карты
            //_gameState.Maps.Add();
            //_gameState.Buildings.Add(newBuildingEntityProxy); 
            return true;
        }
    }
}
