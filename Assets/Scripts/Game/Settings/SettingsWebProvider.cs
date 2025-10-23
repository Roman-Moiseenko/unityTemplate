using System.Threading.Tasks;
using Game.State.Root;
using R3;
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
        
        public Observable<LoadingState> LoadGameSettings()
        {
            var state = new LoadingState();

            //TODO WEB!!!
            
            state.TextState = "Загружаем настройки игры";

            state.Loaded = true;

            return Observable.Return(state);
        }
    }
}