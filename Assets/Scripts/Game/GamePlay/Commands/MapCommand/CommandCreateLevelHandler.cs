﻿using System.Collections.Generic;
using System.Linq;
using Game.Settings;
using Game.State.Entities;
using Game.State.Maps;
using Game.State.Maps.Castle;
using Game.State.Maps.Grounds;
using Game.State.Maps.Roads;
using Game.State.Maps.Towers;
using Game.State.Mergeable.Buildings;
using Game.State.Root;
using MVVM.CMD;
using Newtonsoft.Json;
using R3;
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
            
            foreach (var ground in newMapInitialStateSettings.Grounds)
            {
                var initialGround = new GroundEntityData
                {
                    UniqueId = _gameplayState.CreateEntityID(),
                    ConfigId = newMapInitialStateSettings.GroundDefault,
                    Position = ground.Position,
                    Enabled = ground.Enabled,
                    
                };
                _gameplayState.Grounds.Add(new GroundEntity(initialGround));
            }
            //Рисуем дорогу
            foreach (var road in newMapInitialStateSettings.WayMain)
            {
                var initialRoad = new RoadEntityData
                {
                    UniqueId = _gameplayState.CreateEntityID(),
                    Position = road.Position,
                    ConfigId = newMapInitialStateSettings.RoadDefault,
                    Rotate = road.Rotate,
                    IsTurn = road.IsTurn,
                };
                _gameplayState.Way.Add(new RoadEntity(initialRoad));// Entities.Add(EntitiesFactory.CreateEntity(initialRoad));
            }
            foreach (var road in newMapInitialStateSettings.WaySecond)
            {
                var initialRoad = new RoadEntityData
                {
                    UniqueId = _gameplayState.CreateEntityID(),
                    Position = road.Position,
                    ConfigId = newMapInitialStateSettings.RoadDefault,
                    Rotate = road.Rotate,
                    IsTurn = road.IsTurn,
                };
                _gameplayState.WaySecond.Add(new RoadEntity(initialRoad));// Entities.Add(EntitiesFactory.CreateEntity(initialRoad));
            }
            
            foreach (var road in newMapInitialStateSettings.WayDisabled)
            {
                var initialRoad = new RoadEntityData
                {
                    UniqueId = _gameplayState.CreateEntityID(),
                    Position = road.Position,
                    ConfigId = newMapInitialStateSettings.RoadDefault,
                    Rotate = road.Rotate,
                    IsTurn = road.IsTurn,
                };
                _gameplayState.WayDisabled.Add(new RoadEntity(initialRoad));// Entities.Add(EntitiesFactory.CreateEntity(initialRoad));
            }
            
            
            //Добавляем Волны и Список врагов, по Волнам
            
            //Загружаем базовые параметры
            //Создаем замок
            var castle = new CastleEntityData()
            {
                ConfigId = "Castle",
                Position = new Vector2Int(0, 0),
                Type = EntityType.Building,
                UniqueId = _gameplayState.CreateEntityID(),
                //Базовые параметры
                Damage = _gameSettings.CastleInitialSettings.Damage,
                FullHealth = _gameSettings.CastleInitialSettings.FullHealth,
                DistanceDamage = _gameSettings.CastleInitialSettings.DistanceDamage,
                ReduceHealth = _gameSettings.CastleInitialSettings.ReduceHealth,
                CurrenHealth = _gameSettings.CastleInitialSettings.FullHealth,
                Level = 0,
            };
            _gameplayState.Castle.Value = new CastleEntity(castle);
            
            foreach (var towerSettings in newMapInitialStateSettings.Towers) //Берем список зданий из настроек карты (конфиг)
            {
                var initialTower = new TowerEntityData // .. и создаем все здания
                {
                    UniqueId = _gameplayState.CreateEntityID(),
                    ConfigId = towerSettings.ConfigId,
                    Type = EntityType.Tower,
                    Position = towerSettings.Position,
                    Level = 1,
                };
                _gameplayState.Entities.Add(EntitiesFactory.CreateEntity(initialTower));
            }
            
            return true;
        }
    }
}