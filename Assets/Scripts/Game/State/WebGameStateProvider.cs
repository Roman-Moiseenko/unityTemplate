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
        private const string GAME_STATE_KEY = nameof(GAME_STATE_KEY);
        private const string GAME_SETTINGS_STATE_KEY = nameof(GAME_SETTINGS_STATE_KEY);


        private readonly Coroutines _coroutines;
        public DefaultGameState DefaultGameState { get; private set; } = new();
        public GameStateProxy GameState { get; private set; }
        public GameSettingsStateProxy SettingsState { get; private set; }
        public GameplayStateProxy GameplayState { get; private set; }

        private ReactiveProperty<string> _response = new();
        //  private readonly WebService _webService;

        public ReactiveProperty<bool> WebAvailable = new(false);

        public WebGameStateProvider()
        {
            _coroutines = GameObject.Find("[COROUTINES]").GetComponent<Coroutines>();
            // _webService = new WebService();
//            WebAvailable.Subscribe(v => Debug.Log("Web = " + v));
        }

        public Observable<LoadingState> CheckWebAvailable()
        {
            var state = new LoadingState();
            _coroutines.StartCoroutine(CheckWeb(state));
            return Observable.Return(state);
        }

        public Observable<LoadingState> LoadGameState()
        {
            var state = new LoadingState();
            _coroutines.StartCoroutine(LoadGameStateFromWeb(state));
            return Observable.Return(state);
        }

        public Observable<bool> SaveGameState()
        {
            if (WebAvailable.CurrentValue)
            {
                //TODO Проверка частоты сохранения, отправление данных в пул
                SaveGameStateWeb().Where(x => !x).Subscribe(_ =>
                {
//                    Debug.Log("Данные об игроке не сохранились на сервере");
                    //Данные не сохранились на сервере
                });
            }

            SaveGameStateLocal();
            return Observable.Return(true);
        }

        public Observable<bool> SaveGameStateWeb()
        {
            var Result = false;
            Action<bool> callback = v => { Result = v; };
            var json = JsonConvert.SerializeObject(GameState.Origin);
            
            var formData = new WWWForm();
            formData.AddField("data", json);
            _coroutines.StartCoroutine(SaveDataToServer(WebConstants.WEB_USER_GAMEDATA, formData, callback));

            return Observable.Return(Result);
        }

        public Observable<bool> SaveGameStateLocal()
        {
            if (GameplayState != null) SaveGameplayState();
            var gameJson = JsonConvert.SerializeObject(GameState.Origin, Formatting.Indented);
            PlayerPrefs.SetString(GAME_STATE_KEY, gameJson);
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
            if (WebAvailable.CurrentValue)
            {
                //TODO Проверка частоты сохранения, отправление данных в пул
                SaveSettingsStateWeb().Where(x => !x).Subscribe(_ =>
                {
                    //Данные не сохранились на сервере
                });
            }

            SaveSettingsStateLocal();
            return Observable.Return(true);
        }

        private Observable<bool> SaveSettingsStateWeb()
        {
            var Result = false;
            Action<bool> callback = v => { Result = v; };

            var json = JsonConvert.SerializeObject(SettingsState.Origin);
            var formData = new WWWForm();

            formData.AddField("data", json);
            _coroutines.StartCoroutine(SaveDataToServer(WebConstants.WEB_USER_SETTINGS, formData, callback));
            return Observable.Return(Result);
        }

        private Observable<bool> SaveSettingsStateLocal()
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
                if (!WebAvailable.CurrentValue) //Для первого запуска нужен интернет
                {
                    state.Set("Нет доступа к сети");
                    yield break;
                }

                userId = Path.GetRandomFileName();
                PlayerPrefs.SetString(AppConstants.USER_ID, userId);
                state.Set("Регистрируем нового пользователя");

                yield return _coroutines.StartCoroutine(LoadTextFromServer(
                    WebConstants.WEB_AUTH,
                    WebConstants.BASE_TOKEN,
                    userId));
                userToken = JsonConvert.DeserializeObject<string>(_response.CurrentValue);
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

            GameSettingsState localSetting;
            //Загружаем локальные настройки
            if (PlayerPrefs.HasKey(GAME_SETTINGS_STATE_KEY))
            {
                var json = PlayerPrefs.GetString(GAME_SETTINGS_STATE_KEY);
                localSetting = JsonConvert.DeserializeObject<GameSettingsState>(json);
            }
            else
            {
                localSetting = DefaultGameState.CreateSettingsStateDefault().Origin;
            }

            if (WebAvailable.CurrentValue)
            {
                yield return LoadSettingsAndCompare(userToken, userId, localSetting);
            }
            else
            {
                SettingsState = new GameSettingsStateProxy(localSetting);
                SaveSettingsStateLocal();
            }

            state.Set("Настройки игрока загружены");
            yield return null;
            state.Loaded = true;
        }

        private IEnumerator LoadSettingsAndCompare(string userToken, string userId, GameSettingsState local)
        {
            yield return _coroutines.StartCoroutine(LoadTextFromServer(WebConstants.WEB_USER_SETTINGS, 
                userToken, userId));
            
            var jsonString = JsonConvert.DeserializeObject<string>(_response.CurrentValue);
            if (!string.IsNullOrEmpty(jsonString))
            {
                var webSetting = JsonConvert.DeserializeObject<GameSettingsState>(jsonString);
                //TODO Проверить когда local.DateVersion пусто
                if (DateTime.Compare(webSetting.DateVersion, local.DateVersion) > 0)
                {
                    SettingsState = new GameSettingsStateProxy(webSetting);
                    SaveSettingsStateLocal();
                    yield break;
                }
            }

            SettingsState = new GameSettingsStateProxy(local);
            SaveSettingsStateWeb();
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

            //Загружаем локальные настройки
            GameState localState;
            if (PlayerPrefs.HasKey(GAME_STATE_KEY))
            {
                var json = PlayerPrefs.GetString(GAME_STATE_KEY);
                localState = JsonConvert.DeserializeObject<GameState>(json);
            }
            else
            {
                localState = DefaultGameState.CreateGameStateFromSettings().Origin;
            }
            
            if (WebAvailable.CurrentValue)
            {
                yield return LoadStateAndCompare(userToken, userId, localState);
            }
            else
            {
                GameState = new GameStateProxy(localState);
                SaveGameStateLocal();
            }

//            Debug.Log(JsonConvert.SerializeObject(GameState.Inventory.Origin, Formatting.Indented));
            state.Set("Настройки игрока загружены");
            yield return null;
            state.Loaded = true;
        }

        private IEnumerator LoadStateAndCompare(string userToken, string userId, GameState local)
        {
            yield return _coroutines.StartCoroutine(LoadTextFromServer(
                WebConstants.WEB_USER_GAMEDATA,
                userToken,
                userId));
            var jsonString = JsonConvert.DeserializeObject<string>(_response.CurrentValue);
            if (!string.IsNullOrEmpty(jsonString))
            {
                var webState = JsonConvert.DeserializeObject<GameState>(jsonString);

                if (DateTime.Compare(webState.DateVersion, local.DateVersion) > 0)
                {
                    GameState = new GameStateProxy(webState);
                    SaveGameStateLocal();
                    yield break;
                }
            }
            GameState = new GameStateProxy(local);
            SaveGameStateWeb();
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
                Debug.Log("web ERROR = " + request.downloadHandler.text + " " + userId);
            }

            request.Dispose();
        }

        private IEnumerator SaveDataToServer(string url, WWWForm formData, Action<bool> callback)
        {
            var userId = PlayerPrefs.GetString(AppConstants.USER_ID);
            var userToken = PlayerPrefs.GetString(AppConstants.USER_TOKEN);
            yield return null;

            formData.AddField("user_id", userId);
            var request = UnityWebRequest.Post(url, formData);
            request.SetRequestHeader("authorization", $"Bearer {userToken}");
            yield return request.SendWebRequest();
            callback(request.result == UnityWebRequest.Result.Success);

        //    if (request.result != UnityWebRequest.Result.Success) Debug.Log("web ERROR = " + request.downloadHandler.text);

            request.Dispose();
        }

        private IEnumerator CheckWeb(LoadingState state)
        {
            state.Set("Проверяем соединение с сервером");
            yield return null;
            var request = UnityWebRequest.Get(WebConstants.WEB_CHECK);
            request.SetRequestHeader("authorization", $"Bearer {WebConstants.BASE_TOKEN}");
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                if (JsonConvert.DeserializeObject<bool>(request.downloadHandler.text))
                {
                    WebAvailable.OnNext(true);
                }
                else
                {
                    state.Set("Ошибка доступа!");
                    yield break;
                }
            }
            else
            {
                WebAvailable.OnNext(false);
            }

            yield return null;
            state.Loaded = true;
            request.Dispose();
        }
    }
}