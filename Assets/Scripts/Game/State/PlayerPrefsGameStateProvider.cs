using System;
using System.Collections.Generic;
using Game.State.GameResources;
using Game.State.Inventory;
//using Game.State.Entities.Buildings;
using Game.State.Maps;
using Game.State.Root;
using Newtonsoft.Json;
using R3;
using UnityEngine;

namespace Game.State
{
    public class PlayerPrefsGameStateProvider : IGameStateProvider
    {
        private const string GAME_STATE_KEY = nameof(GAME_STATE_KEY);
        private const string GAME_SETTINGS_STATE_KEY = nameof(GAME_SETTINGS_STATE_KEY);
        public GameStateProxy GameState { get; private set; }
        public GameSettingsStateProxy SettingsState { get; private set; }
        private GameState _gameStateOrigin; //Оригинальное состояние игры
        private GameSettingsState _settingsStateOrigin;
        
        public Observable<GameStateProxy> LoadGameState()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            }; 
            
            if (!PlayerPrefs.HasKey(GAME_STATE_KEY))
            {
                //throw new Exception("1");
                GameState = CreateGameStateFromSettings();
           //     Debug.Log("Игра загружена из настроек " + JsonConvert.SerializeObject(_gameStateOrigin, Formatting.Indented));
                SaveGameState(); //Сохраняем дефолтное состояние
            }
            else
            {
                //Загружаем
                var json = PlayerPrefs.GetString(GAME_STATE_KEY);
          //      Debug.Log("json = " + json);
                _gameStateOrigin = JsonConvert.DeserializeObject<GameState>(json);
          //      Debug.Log("_gameStateOrigin = " + _gameStateOrigin);
                GameState = new GameStateProxy(_gameStateOrigin);
        //        Debug.Log("Игра Загружена " + json);
            }
            return Observable.Return(GameState);
        }

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

        private GameStateProxy CreateGameStateFromSettings()
        {
            _gameStateOrigin = new GameState
            {
                Maps = new List<MapData>(),
                Resources = new List<ResourceData>()
                {
                    new() { Amount = 0, ResourceType = ResourceType.SoftCurrency },
                    new() { Amount = 0, ResourceType = ResourceType.HardCurrency },
                },
                Inventory = new List<InventoryData>(),
            };
       //     Debug.Log("_gameStateOrigin = " + JsonUtility.ToJson(_gameStateOrigin));

            return new GameStateProxy(_gameStateOrigin);
        }

        public Observable<bool> SaveGameState()
        {
            var json = JsonConvert.SerializeObject(_gameStateOrigin, Formatting.Indented);
            PlayerPrefs.SetString(GAME_STATE_KEY, json);
            return Observable.Return(true);
        }

        public Observable<bool> SaveSettingsState()
        {
            var json = JsonConvert.SerializeObject(_settingsStateOrigin, Formatting.Indented);
            PlayerPrefs.SetString(GAME_SETTINGS_STATE_KEY, json);
            return Observable.Return(true);
        }

        public Observable<bool> ResetGameState()
        {
            GameState = CreateGameStateFromSettings();
            SaveGameState();
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