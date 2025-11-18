using System;
using System.Collections;
using System.Threading.Tasks;
using Game.Common;
using Game.GameRoot.Services;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State.Maps.Towers;
using Game.State.Root;
using Newtonsoft.Json;
using R3;
using Scripts.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Settings
{
    public class SettingsProviderWeb : ISettingsProvider
    {
        private const string GAME_SETTINGS_KEY = nameof(GAME_SETTINGS_KEY);
        private const string GAME_SETTINGS_VERSION = nameof(GAME_SETTINGS_VERSION);

        public GameSettings GameSettings => _gameSettings;
        public ApplicationSettings ApplicationSettings { get; }
        private GameSettings _gameSettings;
        private readonly WebService _webService;
        private readonly Coroutines _coroutines;
        private ReactiveProperty<string> _response = new();
        public ReactiveProperty<bool> WebAvailable = new(false);
        private DateTime webDate;

        public SettingsProviderWeb()
        {
            ApplicationSettings = Resources.Load<ApplicationSettings>("ApplicationSettings");
            _webService = new WebService();
            _coroutines = GameObject.Find("[COROUTINES]").GetComponent<Coroutines>();
        }

        public Observable<LoadingState> LoadGameSettings()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            };
            var state = new LoadingState();
            
            _coroutines.StartCoroutine(StartLoadingProcess(state));
            /*
            _response.Skip(1).Subscribe(v =>
            {
                state.Set("Конвертируем полученные настройки игры");
                _gameSettings = JsonConvert.DeserializeObject<GameSettings>(v);
                state.Loaded = true;
            });
*/
            return Observable.Return(state);
        }


        private IEnumerator StartLoadingProcess(LoadingState state)
        {
            state.Set("Проверяем настройки игры");
            var userId = PlayerPrefs.GetString(AppConstants.USER_ID);
            var userToken = PlayerPrefs.GetString(AppConstants.USER_TOKEN);
            yield return null;

            yield return CheckWeb(state); //Запускаем корутину Которая проверяет соединение,

            if (!WebAvailable.CurrentValue) //Связи нет
            {
                if (!PlayerPrefs.HasKey(GAME_SETTINGS_KEY))
                {
                    state.Set("Нет данных по настройкам. Зайдите позже");
                    throw new Exception("Нет данных по настройкам");
                }
                yield return LoadLocalSettings(state);
                yield break;
            }

            yield return SettingsVersion(state); //Загружаем с сервера номер версии настроек

            if (PlayerPrefs.HasKey(GAME_SETTINGS_VERSION)) //Локальная версия сохранена
            {
                var localDate = JsonConvert
                    .DeserializeObject<DateTime>(PlayerPrefs.GetString(GAME_SETTINGS_VERSION));
                if (DateTime.Compare(webDate, localDate) <= 0) //Версия не обновилась
                {
                    yield return LoadLocalSettings(state);
                    yield break;
                }
            }
            //Настройки не загружены или версия не совпадает
            state.Set("Загружаем настройки с сервера");
            yield return LoadTextFromServer(WebConstants.WEB_SETTINGS, userToken, userId);
            state.Set("Конвертируем полученные настройки игры");
            _gameSettings = JsonConvert.DeserializeObject<GameSettings>(_response.Value);
            yield return null;
            state.Set("Сохраняем настройки локально");
            PlayerPrefs.SetString(GAME_SETTINGS_KEY, JsonConvert.SerializeObject(_response.Value));
            PlayerPrefs.SetString(GAME_SETTINGS_VERSION, JsonConvert.SerializeObject(webDate));
            yield return null;
            //TODO После загрузки с сервера создаем список изображений
            //yield return LoadImageFromSettings(state);
            state.Loaded = true;
            


            //yield return LoadTextFromServer(WebConstants.WEB_SETTINGS, userToken, userId);
        }
        
        private IEnumerator LoadLocalSettings(LoadingState state)
        {
            state.Set("Загружаем локальные настройки");
            yield return null;
            var json = JsonConvert.DeserializeObject<string>(PlayerPrefs.GetString(GAME_SETTINGS_KEY));
            _gameSettings = JsonConvert.DeserializeObject<GameSettings>(json);
            state.Loaded = true;
        }
        private IEnumerator LoadSettingsFromWeb(LoadingState state)
        {
            yield return null;
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
            request.Dispose();
        }

        private IEnumerator SettingsVersion(LoadingState state)
        {
            state.Set("Проверяем версию настроек игры");
            yield return null;
            var request = UnityWebRequest.Get(WebConstants.WEB_SETTINGS_VERSION);
            request.SetRequestHeader("authorization", $"Bearer {WebConstants.BASE_TOKEN}");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                webDate = JsonConvert.DeserializeObject<DateTime>(request.downloadHandler.text);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
            }

            yield return null;
            request.Dispose();
        }
    }
}