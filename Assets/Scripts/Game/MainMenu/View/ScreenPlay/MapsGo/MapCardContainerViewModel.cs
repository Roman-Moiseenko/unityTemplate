using DI;
using Game.MainMenu.Services;
using Game.State;
using Game.State.Root;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.MainMenu.View.ScreenPlay.MapsGo
{
    public class MapCardContainerViewModel
    {
        private readonly DIContainer _container;

        public IObservableCollection<MapCardViewModel> AllMapsViewModels;
        public ReactiveProperty<int> LastMapId;
        public MapCardContainerViewModel(GameStateProxy gameState, DIContainer container)
        {
            _container = container;
            var mapCardService = container.Resolve<MapCardService>();
            AllMapsViewModels = mapCardService.AllMaps;
            LastMapId = gameState.MapStates.LastMap;
            
            var lastMap = gameState.MapStates.LastMap.CurrentValue;
            
            foreach (var mapCardViewModel in AllMapsViewModels)
            {
                if (lastMap + 1 >= mapCardViewModel.MapId) mapCardViewModel.EnabledMapCard();
            }
            
/*
            gameState.MapStates.LastMap.Skip(1).Subscribe(newMapId =>
            {
                Debug.Log("newMapId = " + newMapId);
                
                foreach (var mapsViewModel in AllMapsViewModels)
                {
                    if (newMapId + 1 != mapsViewModel.MapId) continue;
                    mapsViewModel.EnabledMapCard();
                    break;
                } 
            });
            */
            //Загружаем список карт из настроек
            //Создаем ViewModel карты и в список

            //Загружаем прогресс Игрока (текущую карту)
        }
    }
}