﻿using System.Collections.Generic;
using Game.State.Maps.Castle;
using Game.State.Maps.Grounds;
using Game.State.Maps.Roads;
using Game.State.Maps.Towers;
using Game.State.Maps.Waves;
using Game.State.Root;
using R3;

namespace Game.State
{
    public class DefaultGameState
    {
        protected GameState _gameStateOrigin; //Оригинальное состояние игры
        protected GameSettingsState _settingsStateOrigin;
        protected GameplayState _gameplayStateOrigin; //Оригинальное состояние игры
        
        
        public GameplayStateProxy CreateGameplayStateFromSettings()
        {
            //Заполняем карты по умолчанию, и другие бонусы
            _gameplayStateOrigin = new GameplayState
            {
                Towers = new List<TowerEntityData>(),
                SoftCurrency = 0,
                Progress = 0,
                ProgressLevel = 0,
                CastleData = new CastleEntityData(),
                Way = new List<RoadEntityData>(),
                WaySecond = new List<RoadEntityData>(),
                WayDisabled = new List<RoadEntityData>(),
                Grounds = new List<GroundEntityData>(),
                Waves = new Dictionary<int, WaveEntityData>(),
                //TODO Ресурсы игры
                
            };
            return new GameplayStateProxy(_gameplayStateOrigin);
        }
        
        public GameSettingsStateProxy CreateSettingsStateDefault()
        {
            _settingsStateOrigin = new GameSettingsState
            {
                MusicVolume = 100,
                SFXVolume = 100
            };

            return new GameSettingsStateProxy(_settingsStateOrigin);
        }
        
        public GameStateProxy CreateGameStateFromSettings()
        {
            //Заполняем карты по умолчанию, и другие бонусы
            //  var inventory = new List<InventoryData>(); //TODO Загрузить из настроек
            _gameStateOrigin = new GameState
            {
                GameSpeed = 1
            };
            return new GameStateProxy(_gameStateOrigin);
        }
    }
}