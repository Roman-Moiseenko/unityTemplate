using System.Collections;
using System.Threading.Tasks;
using Game.Common;
using Game.GameRoot.Services;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State.Root;
using Newtonsoft.Json;
using R3;
using Scripts.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Settings
{
    public class SettingsWebProvider : ISettingsProvider
    {
        public GameSettings GameSettings => _gameSettings;
        public ApplicationSettings ApplicationSettings { get; }
        private GameSettings _gameSettings;
        private readonly WebService _webService;
        private readonly Coroutines _coroutines;
        private ReactiveProperty<string> _response = new();
        
        public SettingsWebProvider()
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

            //TODO WEB!!!
            
            _gameSettings = Resources.Load<GameSettings>("GameSettings");
            var t = new GameSettingsWeb();
            t.TowersSettings.AllTowers.Add(new TowerSettingsWeb());
            var s = JsonConvert.SerializeObject(t);
            var d = JsonConvert.DeserializeObject<GameSettingsWeb>(s);
            var formData = new WWWForm();
            formData.AddField("data", s);
            _coroutines.StartCoroutine(_webService.SaveDataToServer(WebConstants.WEB_SETTINGS_TEST, formData));
            //Debug.Log(s);
            state.Set("Загружаем настройки игры");

            state.Loaded = true;

            return Observable.Return(state);
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
        
    }
}