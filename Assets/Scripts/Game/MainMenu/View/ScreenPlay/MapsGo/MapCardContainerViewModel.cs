using DI;
using Game.Settings;
using Game.Settings.Gameplay.Maps;
using Game.State;
using Game.State.GameStates;
using Game.State.Root;
using ObservableCollections;
using R3;

namespace Game.MainMenu.View.ScreenPlay.MapsGo
{
    /**
     * Контейнер для карт, содержит список MapCardViewModel
     */
    public class MapCardContainerViewModel
    {
        private readonly DIContainer _container;
        private readonly GameStateProxy _gameState;

        public ObservableList<MapCardViewModel> AllMapsViewModels = new();
        public ReactiveProperty<int> LastMapId;
        public MapCardContainerViewModel(GameStateProxy gameState, DIContainer container)
        {
            _container = container;
            _gameState = gameState;
            LastMapId = gameState.MapStates.LastMap;
            var lastMap = gameState.MapStates.LastMap.CurrentValue;
            var settings = container.Resolve<ISettingsProvider>().GameSettings.MapsSettings;
            foreach (var settingsMap in settings.Maps)
            {
                var enabled = lastMap + 1 > settingsMap.MapId;
                CreateMapViewModel(settingsMap, enabled);
            }
            
        }
        
        private void CreateMapViewModel(MapSettings settingsMap, bool enabled)
        {
            _gameState.MapStates.Maps.TryGetValue(settingsMap.MapId, out var mapState);
            var mapViewModel = new MapCardViewModel(settingsMap, _container, mapState);
            mapViewModel.Enabled = enabled;
            AllMapsViewModels.Add(mapViewModel);
        }
        
    }
}