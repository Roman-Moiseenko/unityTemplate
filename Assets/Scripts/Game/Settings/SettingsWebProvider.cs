using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Settings
{
    public class SettingsWebProvider : ISettingsProvider
    {
        public GameSettings GameSettings => _gameSettings;
        public ApplicationSettings ApplicationSettings { get; }
        private GameSettings _gameSettings;
        
        public SettingsWebProvider()
        {
            ApplicationSettings = Resources.Load<ApplicationSettings>("ApplicationSettings");
        }
        
        public Task<GameSettings> LoadGameSettings()
        {
            
            //_gameSettings = Resources.Load<GameSettings>("GameSettings");
         //   var req = await UnityWebRequest.Get($"https://yrry.ru").SendWebRequest();
            
            return Task.FromResult(_gameSettings);
        }
    }
}