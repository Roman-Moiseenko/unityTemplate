using System;
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

      //  private GameState _gameStateOrigin; //Оригинальное состояние игры
      //  private GameSettingsState _settingsStateOrigin;
       // private GameplayState _gameplayStateOrigin; //Оригинальное состояние игры
        public DefaultGameState DefaultGameState { get; private set; } = new();

        //Игровая сессия
        public Observable<GameplayStateProxy> LoadGameplayState()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            };
            if (!PlayerPrefs.HasKey(GAMEPLAY_STATE_KEY))
            {
                GameplayState = DefaultGameState.CreateGameplayStateFromSettings();
                SaveGameplayState(); //Сохраняем дефолтное состояние
            }
            else
            {
                //Загружаем
                var json = PlayerPrefs.GetString(GAMEPLAY_STATE_KEY);
                var gameplayStateOrigin = JsonConvert.DeserializeObject<GameplayState>(json);
                GameplayState = new GameplayStateProxy(gameplayStateOrigin);
            }
            Debug.Log(GameplayState);
            return Observable.Return(GameplayState);
        }
/*
        private GameplayStateProxy CreateGameplayStateFromSettings()
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
            };
            return new GameplayStateProxy(_gameplayStateOrigin);
        }
*/
        public Observable<bool> SaveGameplayState()
        {
            var json = JsonConvert.SerializeObject(GameplayState.Origin, Formatting.Indented);
            PlayerPrefs.SetString(GAMEPLAY_STATE_KEY, json);
            return Observable.Return(true);
        }

        public Observable<bool> ResetGameplayState()
        {
            PlayerPrefs.DeleteKey(GAMEPLAY_STATE_KEY);
            return Observable.Return(true);
        }
        
        //Данные игрока и игры
        public Observable<LoadingState> CheckWebAvailable()
        {
            throw new NotImplementedException();
        }

        public Observable<LoadingState> LoadGameState()
        {
            var state = new LoadingState();
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            };

            if (!PlayerPrefs.HasKey(GAME_STATE_KEY))
            {
                GameState = DefaultGameState.CreateGameStateFromSettings();
                SaveGameState(); //Сохраняем дефолтное состояние
            }
            else
            {
                //Загружаем
                var json = PlayerPrefs.GetString(GAME_STATE_KEY);
                var gameStateOrigin = JsonConvert.DeserializeObject<GameState>(json);
                GameState = new GameStateProxy(gameStateOrigin);
            }
            state.Set("Загружаем сохранение игрока");
            state.Loaded = true;
            return Observable.Return(state);
        }
        
        public Observable<bool> SaveGameState()
        {
            Debug.Log(GameplayState);
            if (GameplayState != null) SaveGameplayState();
            
            var gameJson = JsonConvert.SerializeObject(GameState.Origin, Formatting.Indented);
            Debug.Log(gameJson);
            PlayerPrefs.SetString(GAME_STATE_KEY, gameJson);
            
            return Observable.Return(true);
        }

        public Observable<bool> ResetGameState()
        {
            GameState = DefaultGameState.CreateGameStateFromSettings();
            SaveGameState();
            return Observable.Return(true);
        }
        
        //Настройки игры
        public Observable<LoadingState> LoadSettingsState()
        {
            var state = new LoadingState();

            if (!PlayerPrefs.HasKey(GAME_SETTINGS_STATE_KEY))
            {
                SettingsState = DefaultGameState.CreateSettingsStateDefault();
                SaveSettingsState(); //Сохраняем дефолтное состояние
            }
            else
            {
                //Загружаем
                var json = PlayerPrefs.GetString(GAME_SETTINGS_STATE_KEY);
                var settingsStateOrigin = JsonConvert.DeserializeObject<GameSettingsState>(json);
                SettingsState = new GameSettingsStateProxy(settingsStateOrigin);
            }

            state.Set("Загружаем настройки игрока");
            state.Loaded = true;

            return Observable.Return(state);
        }

        public Observable<bool> SaveSettingsState()
        {
            var json = JsonConvert.SerializeObject(SettingsState.Origin, Formatting.Indented);
            PlayerPrefs.SetString(GAME_SETTINGS_STATE_KEY, json);
            return Observable.Return(true);
        }

        public Observable<bool> ResetSettingsState()
        {
            SettingsState = DefaultGameState.CreateSettingsStateDefault();
            SaveSettingsState();
            return Observable.Return(true);
        }
/*
        private GameSettingsStateProxy CreateSettingsStateDefault()
        {
            _settingsStateOrigin = new GameSettingsState
            {
                MusicVolume = 100,
                SFXVolume = 100
            };

            return new GameSettingsStateProxy(_settingsStateOrigin);
        }
        */
    }
}