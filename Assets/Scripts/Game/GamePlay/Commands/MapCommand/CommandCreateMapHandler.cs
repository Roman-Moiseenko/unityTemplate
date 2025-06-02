using System.Collections.Generic;
using System.Linq;
using Game.Settings;
using Game.State.CMD;
using Game.State.Entities;
using Game.State.Maps;
using Game.State.Mergeable.Buildings;
using Game.State.Root;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.GamePlay.Commands.MapCommand
{
    public class CommandCreateMapHandler
        : ICommandHandler<CommandCreateMap>
    {
        private readonly GameStateProxy _gameState;
        private readonly GameSettings _gameSettings;

        public CommandCreateMapHandler(GameStateProxy gameState, GameSettings gameSettings)
        {
            _gameState = gameState;
            _gameSettings = gameSettings;
        }

        public bool Handle(CommandCreateMap command)
        {
            Debug.Log("Создаем карту - ");
            var isMapAlreadyExisted = _gameState.Maps.Any(m => m.Id == command.MapId);
            if (isMapAlreadyExisted) //Если карта была создано, то ошибка
            {
                Debug.Log($"Map id={command.MapId} already exist");
                return false;
            }
            //Находим настройки карты по ее Id
            var newMapSettings = _gameSettings.MapsSettings.Maps.First(m => m.MapId == command.MapId);
            var newMapInitialStateSettings = newMapSettings.InitialStateSettings;
            var initialEntities = new List<EntityData>(); //Создаем список зданий

            Debug.Log("newMapSettings " + JsonConvert.SerializeObject(newMapSettings, Formatting.Indented));
            foreach (var buildingSettings in newMapInitialStateSettings.Buildings) //Берем список зданий из настроек карты (конфиг)
            {
                var initialBuilding = new BuildingEntityData // .. и создаем все здания
                {
                    UniqueId = _gameState.CreateEntityID(),
                    ConfigId = buildingSettings.TypeId,
                    Type = EntityType.Building,
                    Position = buildingSettings.Position,
                    Level = buildingSettings.Level,
                    IsAutoCollectionEnabled = false,
                    LastClickedTimeMS = 0,
                };
                initialEntities.Add(initialBuilding);
            }
            
            //Создаем другие ресурсы карты 
            /// ..... 
            
            //Создаем состояние карты
            var newMapState = new MapData
            {
                Id = command.MapId,
                Entities = initialEntities,
             //   Buildings = initialBuildings,
            };
            // ... затем оборачиваем прокис
            var newMapStateProxy = new Map(newMapState);
            _gameState.Maps.Add(newMapStateProxy); //И добавляем в список карт игры
            return true;
        }
    }
}