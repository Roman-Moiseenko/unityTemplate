using System.Linq;
using Game.GamePlay.Commands.TowerCommand;
using Game.State.Maps.Towers;
using Game.State.Root;
using MVVM.CMD;
using Newtonsoft.Json;
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
            var entityId = _gameplayState.CreateEntityID(); //Получаем уникальный ID
            var newTowerEntity = new TowerEntityData() //Создаем сущность игрового объекта
            {
                UniqueId = entityId,
                Position = command.Position,
                ConfigId = command.TowerTypeId,
            };
            var newTower = new TowerEntity(newTowerEntity); //Оборачиваем его Прокси
            newTower.IsFrame = true; //Сначала создаем каркас башни
            _gameplayState.Entities.Add(newTower);//Добавляем в список объектов карты
            return false;
        }
    }
}
