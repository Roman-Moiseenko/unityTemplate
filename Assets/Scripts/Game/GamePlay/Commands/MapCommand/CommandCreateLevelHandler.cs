using System.Collections.Generic;
using System.Linq;
using Game.Settings;
using Game.State.CMD;
using Game.State.Entities;
using Game.State.Maps;
using Game.State.Maps.Grounds;
using Game.State.Maps.Towers;
using Game.State.Mergeable.Buildings;
using Game.State.Root;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.GamePlay.Commands.MapCommand
{
    public class CommandCreateLevelHandler
        : ICommandHandler<CommandCreateLevel>
    {
        
        private readonly GameSettings _gameSettings;
        private readonly GameplayStateProxy _gameplayState;

        public CommandCreateLevelHandler(GameSettings gameSettings, GameplayStateProxy gameplayState)
        {
           
            _gameSettings = gameSettings;
            _gameplayState = gameplayState;
        }

        public bool Handle(CommandCreateLevel command)
        {
            if (_gameplayState.Entities.Any())
            {
                Debug.Log($"Map id={command.MapId} already exist");
                return false;
            }
           /* var isMapAlreadyExisted = _gameState.Maps.Any(m => m.Id == command.MapId);
            if (isMapAlreadyExisted) //Если карта была создана, то ошибка
            {

            } */
            //Находим настройки карты по ее Id
            var newMapSettings = _gameSettings.MapsSettings.Maps.First(m => m.MapId == command.MapId);
            
            var newMapInitialStateSettings = newMapSettings.InitialStateSettings;
           // var initialEntities = new List<EntityData>(); //Создаем список зданий
            
            foreach (var ground in newMapInitialStateSettings.Grounds)
            {
                var initialGround = new GroundEntityData
                {
                    Type = EntityType.Ground,
                    UniqueId = _gameplayState.CreateEntityID(),
                    ConfigId = newMapInitialStateSettings.GroundDefault,
                    Position = ground.Position,
                    Enabled = ground.Enabled,
                    
                };
                _gameplayState.Entities.Add(EntitiesFactory.CreateEntity(initialGround));
                //initialEntities.Add(initialGround);
                
            }
            //Рисуем дорогу
            
            //Добавляем Волны и Список врагов, по Волнам
            
            
            foreach (var buildingSettings in newMapInitialStateSettings.Buildings) //Берем список зданий из настроек карты (конфиг)
            {
                var initialBuilding = new BuildingEntityData // .. и создаем все здания
                {
                    UniqueId = _gameplayState.CreateEntityID(),
                    ConfigId = buildingSettings.ConfigId,
                    Type = EntityType.Building,
                    Position = buildingSettings.Position,
                    Level = buildingSettings.Level,
                    IsAutoCollectionEnabled = false,
                    LastClickedTimeMS = 0,
                };
                _gameplayState.Entities.Add(EntitiesFactory.CreateEntity(initialBuilding));
              //  initialEntities.Add(initialBuilding);
            }
            Debug.Log("newMapInitialStateSettings.Towers " + JsonConvert.SerializeObject(newMapInitialStateSettings.Towers, Formatting.Indented));
            foreach (var towerSettings in newMapInitialStateSettings.Towers) //Берем список зданий из настроек карты (конфиг)
            {
                var initialTower = new TowerEntityData // .. и создаем все здания
                {
                    UniqueId = _gameplayState.CreateEntityID(),
                    ConfigId = towerSettings.ConfigId,
                    Type = EntityType.Tower,
                    Position = towerSettings.Position,
                    EpicLevel = towerSettings.Level,
                };
                _gameplayState.Entities.Add(EntitiesFactory.CreateEntity(initialTower));
              //  initialEntities.Add(initialTower);
            }
            
/*
            //Создаем состояние карты
            var newMapState = new MapData
            {
                Id = command.MapId,
                Entities = initialEntities,
            };
            // ... затем оборачиваем прокис
            var newMapStateProxy = new Map(newMapState);
            _gameplayState.Entities.Add(newMapStateProxy); //И добавляем в список карт игры 
            */
            return true;
        }
    }
}