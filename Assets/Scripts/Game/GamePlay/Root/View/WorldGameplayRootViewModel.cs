using Game.GamePlay.Services;
using Game.GamePlay.View.Buildings;
using Game.GamePlay.View.Grounds;
using Game.GamePlay.View.Towers;
using Game.State.GameResources;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.Root.View
{
    public class WorldGameplayRootViewModel
    {
        private readonly ResourcesService _resourcesService;
        public readonly IObservableCollection<BuildingViewModel> AllBuildings;
        public readonly IObservableCollection<TowerViewModel> AllTowers;
        public readonly IObservableCollection<GroundViewModel> AllGrounds;
 
        public WorldGameplayRootViewModel(
            BuildingsService buildingsService,
            GroundsService groundsService,
            ResourcesService resourcesService,
            TowersService towersService
            )
        {
            _resourcesService = resourcesService;
            
            AllBuildings = buildingsService.AllBuildings;
            AllGrounds = groundsService.AllGrounds;
            AllTowers = towersService.AllTowers;

            resourcesService.ObservableResource(ResourceType.SoftCurrency).Subscribe(
                newValue => Debug.Log($"SoftCurrency = {newValue}"));
            
            resourcesService.ObservableResource(ResourceType.HardCurrency).Subscribe(
                newValue => Debug.Log($"HardCurrency = {newValue}"));
        }


        public void ControllInput(Vector3Int position, string ConfigId )
        {
            
        }

    }
}