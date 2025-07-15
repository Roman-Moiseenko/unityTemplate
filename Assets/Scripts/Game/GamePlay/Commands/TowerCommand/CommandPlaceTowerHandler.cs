using System.Linq;
using Game.GamePlay.Commands.TowerCommand;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State.Entities;
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
        private readonly TowersSettings _towersSettings;

        public CommandPlaceTowerHandler(GameplayStateProxy gameplayState,
            TowersSettings towersSettings)
        {
            _gameplayState = gameplayState;
            _towersSettings = towersSettings;
        }
        
        public bool Handle(CommandPlaceTower command)
        {
            var entityId = _gameplayState.CreateEntityID(); //Получаем уникальный ID
            var towerSettings = _towersSettings.AllTowers.Find(t => t.ConfigId == command.TowerTypeId);
            
            var newTowerEntity = new TowerEntityData() //Создаем сущность игрового объекта
            {
                UniqueId = entityId,
                Position = command.Position,
                ConfigId = command.TowerTypeId,
                Type = EntityType.Tower,
                TypeEnemy = towerSettings.TypeEnemy,
                IsMultiShot = towerSettings.MultiShot,
                IsOnRoad = towerSettings.OnRoad,
            };
            var newTower = new TowerEntity(newTowerEntity); //Оборачиваем его Прокси
            _gameplayState.Towers.Add(newTower);//Добавляем в список объектов карты
            return true;
        }
    }
}
