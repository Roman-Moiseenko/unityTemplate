using DI;
using Game.MainMenu.View.ScreenPlay.MapsGo;
using Game.Settings.Gameplay.Maps;
using Game.State.GameStates;
using Game.State.Root;
using MVVM.CMD;
using ObservableCollections;
using UnityEngine;

namespace Game.MainMenu.Services
{
    public class MapCardService
    {
        private readonly ICommandProcessor _cmd;
        private readonly DIContainer _container;
        private readonly MapsSettings _settings;
        private readonly GameStateProxy _gameState;
        private readonly ObservableList<MapCardViewModel> _allMaps = new();
        public IObservableCollection<MapCardViewModel> AllMaps =>
            _allMaps; //Интерфейс менять нельзя, возвращаем через динамический массив

        public MapCardService(
            GameStateProxy gameState,
            MapsSettings settings,
            ICommandProcessor cmd,
            DIContainer container 
        )
        {
            _cmd = cmd;
            _container = container;
            _settings = settings;
            _gameState = gameState;
//            Debug.Log(_gameState.MapStates.LastMap.CurrentValue);
/*
            foreach (var settingsMap in _settings.Maps)
            {
//                Debug.Log(settingsMap.MapId);
                CreateMapViewModel(settingsMap);
            }
            */
            //TODO Отслеживание прогресса игрока, 
        }
/*
        private void CreateMapViewModel(MapSettings settingsMap)
        {
            MapState mapState;

            _gameState.MapStates.Maps.TryGetValue(settingsMap.MapId, out mapState);
            
            var mapViewModel = new MapCardViewModel(settingsMap, _container, mapState);
            _allMaps.Add(mapViewModel);
        }
        */
    }
}