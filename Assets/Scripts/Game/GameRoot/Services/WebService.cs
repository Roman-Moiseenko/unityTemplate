using System;
using System.Collections;
using Game.Common;
using Newtonsoft.Json;
using R3;
using Scripts.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.GameRoot.Services
{
    public class WebService
    {
        
        private readonly Coroutines _coroutines;

        public WebService()
        {
            _coroutines = GameObject.Find("[COROUTINES]").GetComponent<Coroutines>();
        }

        public IEnumerator CheckWeb(Action<bool> callback)
        {
            var request = UnityWebRequest.Get(WebConstants.WEB_CHECK);
            request.SetRequestHeader("authorization", $"Bearer {WebConstants.BASE_TOKEN}");
            yield return request.SendWebRequest();
            callback(request.result == UnityWebRequest.Result.Success);
            
            request.Dispose();
        }

        public IEnumerator LoadTextFromServer(string url, string token, string userId, Action<string> callback)
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            };

            var formData = new WWWForm();
            formData.AddField("user_id", userId);

            var urlParam = $"{url}/?user_id={userId}";
            var request = UnityWebRequest.Get(urlParam);
            request.SetRequestHeader("authorization", $"Bearer {token}");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                callback(request.downloadHandler.text);
                //response.Value = request.downloadHandler.text;
            }
            else
            {
                callback("");
                Debug.Log("web ERROR = " + request.downloadHandler.text);
            }

            request.Dispose();
        }

        public IEnumerator SaveDataToServer(string url, WWWForm formData)
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
    
        
        
        public bool FirstAuthorization(string userId)
        {
            var request = new UnityWebRequest(WebConstants.WEB_AUTH);
            
            //WebRequest(request, null, null);
            
            
            return true;
        }
        
        //public void Load
    }
}