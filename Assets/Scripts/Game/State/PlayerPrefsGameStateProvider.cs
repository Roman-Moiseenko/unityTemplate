using System;
using System.Collections.Generic;
using Game.State.Entities;
using Game.State.GameResources;
using Game.State.Inventory;
//using Game.State.Entities.Buildings;
using Game.State.Maps;
using Game.State.Maps.Castle;
using Game.State.Maps.Grounds;
using Game.State.Maps.Roads;
using Game.State.Root;
using Newtonsoft.Json;
using R3;
using UnityEngine;

namespace Game.State
{
    public class PlayerPrefsGameStateProvider : IGameStateProvider
    {
        private const string GAME_STATE_KEY = nameof(GAME_STATE_KEY);
        private const string GAMEPLAY_STATE_KEY = nameof(GAMEPLAY_STATE_KEY);
        private const string GAME_SETTINGS_STATE_KEY = nameof(GAME_SETTINGS_STATE_KEY);
        public GameStateProxy GameState { get; private set; }
        public GameSettingsStateProxy SettingsState { get; private set; }
        public GameplayStateProxy GameplayState { get; private set; }
        
        private GameState _gameStateOrigin; //Оригинальное состояние игры
        private GameSettingsState _settingsStateOrigin;
        private GameplayState _gameplayStateOrigin; //Оригинальное состояние игры
        
        //Игровая сессия
        public Observable<GameplayStateProxy> LoadGameplayState()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            }; 
//            Debug.Log("!PlayerPrefs.HasKey(GAMEPLAY_STATE_KEY) = "  + !PlayerPrefs.HasKey(GAMEPLAY_STATE_KEY));
            
            if (!PlayerPrefs.HasKey(GAMEPLAY_STATE_KEY))
            {
                GameplayState = CreateGameplayStateFromSettings();
                Debug.Log("_gameplayStateOrigin = " + JsonConvert.SerializeObject(_gameplayStateOrigin, Formatting.Indented));

                SaveGameplayState(); //Сохраняем дефолтное состояние
            }
            else
            {
                //Загружаем
                var json = PlayerPrefs.GetString(GAMEPLAY_STATE_KEY);
                _gameplayStateOrigin = JsonConvert.DeserializeObject<GameplayState>(json);
                Debug.Log(JsonConvert.SerializeObject(_gameplayStateOrigin, Formatting.Indented));
                GameplayState = new GameplayStateProxy(_gameplayStateOrigin);
                Debug.Log("_gameplayStateOrigin = " + JsonConvert.SerializeObject(_gameplayStateOrigin, Formatting.Indented));
            }
            return Observable.Return(GameplayState);
        }
        private GameplayStateProxy CreateGameplayStateFromSettings()
        {
            //Debug.Log("CreateGameplayStateFromSettings" );
            //Заполняем карты по умолчанию, и другие бонусы
            _gameplayStateOrigin = new GameplayState
            {
                Entities = new List<EntityData>(),
                SoftCurrency = 0,
                Progress = 0,
                ProgressLevel = 0,
                CastleData = new CastleEntityData(),
                Way = new List<RoadEntityData>(),
                WaySecond = new List<RoadEntityData>(),
                WayDisabled = new List<RoadEntityData>(),
                Grounds = new List<GroundEntityData>()
                
                //TODO Ресурсы игры
            };
            //_gameStateOrigin.GameplayStateData.GameSpeed = 1;
            //     Debug.Log("_gameStateOrigin = " + JsonUtility.ToJson(_gameStateOrigin));

            return new GameplayStateProxy(_gameplayStateOrigin);
        }
        public Observable<bool> SaveGameplayState()
        {
            var json = JsonConvert.SerializeObject(_gameplayStateOrigin, Formatting.Indented);
            PlayerPrefs.SetString(GAMEPLAY_STATE_KEY, json);
            return Observable.Return(true);
        }
        public Observable<bool> ResetGameplayState()
        {
            Debug.Log("ResetGameplayState" );

            PlayerPrefs.DeleteKey(GAMEPLAY_STATE_KEY);
          //  GameplayState = CreateGameplayStateFromSettings();
          //  SaveGameplayState();
            return Observable.Return(true);
        }

        
        //Данные игрока и игры
        public Observable<GameStateProxy> LoadGameState()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            }; 
            
            if (!PlayerPrefs.HasKey(GAME_STATE_KEY))
            {
                GameState = CreateGameStateFromSettings();
                SaveGameState(); //Сохраняем дефолтное состояние
            }
            else
            {
                //Загружаем
                var json = PlayerPrefs.GetString(GAME_STATE_KEY);
                _gameStateOrigin = JsonConvert.DeserializeObject<GameState>(json);
                GameState = new GameStateProxy(_gameStateOrigin);
            }
            return Observable.Return(GameState);
        }
        private GameStateProxy CreateGameStateFromSettings()
        {
            //Заполняем карты по умолчанию, и другие бонусы
            var inventory = new List<InventoryData>(); //TODO Загрузить из настроек
            
            
            _gameStateOrigin = new GameState
            {
                CurrentMapId = 0,
                Resources = new List<ResourceData>()
                {
                    new() { Amount = 0, ResourceType = ResourceType.SoftCurrency },
                    new() { Amount = 0, ResourceType = ResourceType.HardCurrency },
                },
                Inventory = inventory,  
            };
            _gameStateOrigin.GameSpeed = 1;
       //     Debug.Log("_gameStateOrigin = " + JsonUtility.ToJson(_gameStateOrigin));

            return new GameStateProxy(_gameStateOrigin);
        }
        public Observable<bool> SaveGameState()
        {
            var json = JsonConvert.SerializeObject(_gameStateOrigin, Formatting.Indented);
            PlayerPrefs.SetString(GAME_STATE_KEY, json);
            return Observable.Return(true);
        }
        public Observable<bool> ResetGameState()
        {
            GameState = CreateGameStateFromSettings();
            SaveGameState();
            return Observable.Return(true);
        }
        
        
        //Настройки игры
        public Observable<GameSettingsStateProxy> LoadSettingsState()
                {
                //    throw new System.NotImplementedException();
                    
                    if (!PlayerPrefs.HasKey(GAME_SETTINGS_STATE_KEY))
                    {
                        SettingsState = CreateSettingsStateDefault();
                  //      Debug.Log("Настройки установлены по-умолчанию " + JsonUtility.ToJson(_settingsStateOrigin, true));
                        SaveSettingsState(); //Сохраняем дефолтное состояние
                    }
                    else
                    {
                        //Загружаем
                        var json = PlayerPrefs.GetString(GAME_SETTINGS_STATE_KEY);
                        _settingsStateOrigin = JsonConvert.DeserializeObject<GameSettingsState>(json);
                        SettingsState = new GameSettingsStateProxy(_settingsStateOrigin);
                    //    Debug.Log("Настройки Загружены " + json);
                    }
                    return Observable.Return(SettingsState);
                    
                }
        public Observable<bool> SaveSettingsState()
                {
                    var json = JsonConvert.SerializeObject(_settingsStateOrigin, Formatting.Indented);
                    PlayerPrefs.SetString(GAME_SETTINGS_STATE_KEY, json);
                    return Observable.Return(true);
                }
        public Observable<bool> ResetSettingsState()
        {
            SettingsState = CreateSettingsStateDefault();
            SaveSettingsState();
            return Observable.Return(true);
        }
        private GameSettingsStateProxy CreateSettingsStateDefault()
        {
            _settingsStateOrigin = new GameSettingsState
            {
                MusicVolume = 100,
                SFXVolume = 100
            };

            return new GameSettingsStateProxy(_settingsStateOrigin);
        }
    }
}