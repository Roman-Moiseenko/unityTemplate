using System.Threading.Tasks;
using Game.State.Root;
using R3;
using UnityEngine;

namespace Game.Settings
{
    public class SettingsProvider : ISettingsProvider
    {
        public GameSettings GameSettings => _gameSettings;
        public ApplicationSettings ApplicationSettings { get; }
        private GameSettings _gameSettings;
        
        public SettingsProvider()
        {
            ApplicationSettings = Resources.Load<ApplicationSettings>("ApplicationSettings");
        }
        
        public Observable<LoadingState> LoadGameSettings()
        {
            var state = new LoadingState();

            _gameSettings = Resources.Load<GameSettings>("GameSettings");
            
            state.Set("Загружаем настройки игры");

            state.Loaded = true;

            return Observable.Return(state);
        }
    }
}