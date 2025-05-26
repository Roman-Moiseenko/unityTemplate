using System.Collections.Generic;
using System.Linq;
using Game.Settings;
using Game.State.CMD;
using Game.State.Entities.Buildings;
using Game.State.Maps;
using Game.State.Root;
using UnityEngine;

namespace Game.GamePlay.Commands
{
    public class CommandCreateMapStateHandler : ICommandHandler<CommandCreateMapState>
    {
        private readonly GameStateProxy _gameState;
        private readonly GameSettings _gameSettings;

        public CommandCreateMapStateHandler(GameStateProxy gameState, GameSettings gameSettings)
        {
            _gameState = gameState;
            _gameSettings = gameSettings;
        }

        public bool Handle(CommandCreateMapState command)
        {
            var isMapAlreadyExisted = _gameState.Maps.Any(m => m.Id == command.MapId);
            if (isMapAlreadyExisted) //Если карта была создано, то ошибка
            {
                Debug.Log($"Map id={command.MapId} already exist");
                return false;
            }
            //Находим настройки карты по ее Id
            var newMapSettings = _gameSettings.MapsSettings.Maps.First(m => m.MapId == command.MapId);
            var newMapInitialStateSettings = newMapSettings.InitialStateSettings;
            var initialBuildings = new List<BuildingEntity>(); //Создаем список зданий

            foreach (var buildingSettings in newMapInitialStateSettings.Buildings) //Берем список зданий из настроек карты (конфиг)
            {
                var initialBuilding = new BuildingEntity // .. и создаем все здания
                {
                    Id = _gameState.CreateEntityID(),
                    TypeId = buildingSettings.TypeId,
                    Position = buildingSettings.Position,
                    Level = buildingSettings.Level
                };
                initialBuildings.Add(initialBuilding);
            }
            //Создаем другие ресурсы карты 
            /// ..... 
            
            //Создаем состояние карты
            var newMapState = new MapState
            {
                Id = command.MapId,
                Buildings = initialBuildings,
            };
            // ... затем оборачиваем прокис
            var newMapStateProxy = new Map(newMapState);
            _gameState.Maps.Add(newMapStateProxy); //И добавляем в список карт игры
            return true;
        }
    }
}