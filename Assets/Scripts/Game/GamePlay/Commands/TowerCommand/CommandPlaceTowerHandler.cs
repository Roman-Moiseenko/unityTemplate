using System.Linq;
using Game.GamePlay.Commands.TowerCommand;
using Game.State.CMD;
using Game.State.Maps.Towers;
using Game.State.Root;
using UnityEngine;

namespace Game.GamePlay.Commands
{
    public class CommandPlaceTowerHandler : ICommandHandler<CommandPlaceTower>
    {
        private readonly GameStateProxy _gameState;

        public CommandPlaceTowerHandler(GameStateProxy gameState)
        {
            _gameState = gameState;
        }
        
        public bool Handle(CommandPlaceTower command)
        {
            var currentMap = _gameState.Maps.FirstOrDefault(m => m.Id == _gameState.CurrentMapId.CurrentValue);
            if (currentMap == null)
            {
                Debug.Log($" Карта не найдена { _gameState.CurrentMapId.CurrentValue}");
                return false;
            }
            
           // var entityId = _gameState.CreateEntityID(); //Получаем уникальный ID
            var newTowerEntity = new TowerEntityData() //Создаем сущность игрового объекта
            {
                Position = command.Position,
                ConfigId = command.TowerTypeId,
            };
            var newTower = new TowerEntity(newTowerEntity); //Оборачиваем его Прокси
            currentMap.Entities.Add(newTower);//Добавляем в список объектов карты
            //_gameState.Maps.Add();
            //_gameState.Buildings.Add(newBuildingEntityProxy); 
            return true;
        }
    }
}
