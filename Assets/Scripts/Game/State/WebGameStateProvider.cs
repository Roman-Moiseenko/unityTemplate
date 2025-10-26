using System;
using System.Collections;
using System.IO;
using Game.Common;
using Game.GameRoot.Services;
using Game.State.Root;
using Newtonsoft.Json;
using R3;
using Scripts.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.State
{
    public class WebGameStateProvider : IGameStateProvider
    {
        private const string GAMEPLAY_STATE_KEY = nameof(GAMEPLAY_STATE_KEY);

        private readonly Coroutines _coroutines;
        public DefaultGameState DefaultGameState { get; private set; } = new();
        public GameStateProxy GameState { get; private set; }
        public GameSettingsStateProxy SettingsState { get; private set; }
        public GameplayStateProxy GameplayState { get; private set; }

        private ReactiveProperty<string> _response = new();
        private readonly WebService _webService;

        public WebGameStateProvider()
        {
            _coroutines = GameObject.Find("[COROUTINES]").GetComponent<Coroutines>();
            _webService = new WebService();
        }

        public Observable<LoadingState> LoadGameState()
        {
            var state = new LoadingState();
            _coroutines.StartCoroutine(LoadGameStateFromWeb(state));
            return Observable.Return(state);
        }

        public Observable<bool> SaveGameState()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            };
            
            if (GameplayState != null) SaveGameplayState();
            var json = JsonConvert.SerializeObject(GameState.Origin);
            
            var formData = new WWWForm();
            formData.AddField("data", json);
            _coroutines.StartCoroutine(SaveDataToServer(WebConstants.WEB_USER_GAMEDATA, formData));
            return Observable.Return(true);
        }

        public Observable<bool> ResetGameState()
        {
            GameState = DefaultGameState.CreateGameStateFromSettings();
            SaveGameState();
            return Observable.Return(true);
        }

        public Observable<LoadingState> LoadSettingsState()
        {
            var state = new LoadingState();
            _coroutines.StartCoroutine(LoadSettingsFromWeb(state));
            return Observable.Return(state);
        }

        public Observable<bool> SaveSettingsState()
        {
            var json = JsonConvert.SerializeObject(SettingsState.Origin);
            var formData = new WWWForm();
            formData.AddField("data", json);
            _coroutines.StartCoroutine(SaveDataToServer(WebConstants.WEB_USER_SETTINGS, formData));
            return Observable.Return(true);
        }

        public Observable<bool> ResetSettingsState()
        {
            SettingsState = DefaultGameState.CreateSettingsStateDefault();
            SaveSettingsState();
            return Observable.Return(true);
        }

        //Gameplay - Локально
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

        public Observable<bool> SaveGameplayState()
        {
            var json = JsonConvert.SerializeObject(GameplayState.Origin);
            PlayerPrefs.SetString(GAMEPLAY_STATE_KEY, json);
            return Observable.Return(true);
        }

        public Observable<bool> ResetGameplayState()
        {
            PlayerPrefs.DeleteKey(GAMEPLAY_STATE_KEY);
            return Observable.Return(true);
        }
        
        private IEnumerator LoadSettingsFromWeb(LoadingState state)
        {
            string userId;
            string userToken;
            
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            };
            
            if (!PlayerPrefs.HasKey(AppConstants.USER_ID)) //Регистрация нового пользователя
            {
                userId = Path.GetRandomFileName();
                PlayerPrefs.SetString(AppConstants.USER_ID, userId);
                state.Set("Регистрируем нового пользователя");

                yield return _coroutines.StartCoroutine(LoadTextFromServer(
                    WebConstants.WEB_AUTH,
                    WebConstants.BASE_TOKEN,
                    userId));

                userToken = JsonConvert.DeserializeObject<string>(_response.CurrentValue);
                //userToken = jsonString;
                PlayerPrefs.SetString(AppConstants.USER_TOKEN, userToken);
                state.Set("Пользователь зарегистрирован");
                yield return null;
            }
            else
            {
                userId = PlayerPrefs.GetString(AppConstants.USER_ID);
                userToken = PlayerPrefs.GetString(AppConstants.USER_TOKEN);
            }
            
            state.Set("Загружаем настройки игрока");
            yield return null;
            yield return _coroutines.StartCoroutine(LoadTextFromServer(
                WebConstants.WEB_USER_SETTINGS,
                userToken,
                userId));
            var jsonString = JsonConvert.DeserializeObject<string>(_response.CurrentValue);
            if (string.IsNullOrEmpty(jsonString))
            {
                ResetSettingsState();
            }
            else
            {
                var settingsStateOrigin = JsonConvert.DeserializeObject<GameSettingsState>(jsonString);
                SettingsState = new GameSettingsStateProxy(settingsStateOrigin);
            }

            state.Set("Настройки игрока загружены");
            yield return null;
            state.Loaded = true;
        }

        private IEnumerator LoadGameStateFromWeb(LoadingState state)
        {
            state.Set("Загрузка данных игрока");
            yield return null;
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            };
            var userId = PlayerPrefs.GetString(AppConstants.USER_ID);
            var userToken = PlayerPrefs.GetString(AppConstants.USER_TOKEN);

            
            yield return _coroutines.StartCoroutine(LoadTextFromServer(
                WebConstants.WEB_USER_GAMEDATA,
                userToken,
                userId));
            var jsonString = JsonConvert.DeserializeObject<string>(_response.CurrentValue);
            if (string.IsNullOrEmpty(jsonString))
            {
                ResetGameState();
            }
            else
            {
                //Debug.Log(jsonString);
                var gameStateOrigin = JsonConvert.DeserializeObject<GameState>(jsonString);
                GameState = new GameStateProxy(gameStateOrigin);
            }

            state.Set("Настройки игрока загружены");
            yield return null;
            state.Loaded = true;
        }
        

        private IEnumerator LoadTextFromServer(string url, string token, string userId)
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            };
            
            _response.Value = null;
            var formData = new WWWForm();
            formData.AddField("user_id", userId);

            var urlParam = $"{url}/?user_id={userId}";
            var request = UnityWebRequest.Get(urlParam);
            request.SetRequestHeader("authorization", $"Bearer {token}");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                _response.Value = request.downloadHandler.text;
            }
            else
            {
                Debug.Log("web ERROR = " + request.downloadHandler.text);
            }

            request.Dispose();
        }

        private IEnumerator SaveDataToServer(string url, WWWForm formData)
        {
            var userId = PlayerPrefs.GetString(AppConstants.USER_ID);
            var userToken = PlayerPrefs.GetString(AppConstants.USER_TOKEN);
            yield return null;
            
            formData.AddField("user_id", userId);
            var request = UnityWebRequest.Post(url, formData);
            request.SetRequestHeader("authorization", $"Bearer {userToken}");
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
                Debug.Log("web ERROR = " + request.downloadHandler.text);
            

            request.Dispose();
        }
        
    }
}