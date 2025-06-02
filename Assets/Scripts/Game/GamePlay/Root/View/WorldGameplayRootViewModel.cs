using Game.GamePlay.Services;
using Game.GamePlay.View.Buildings;
using Game.GamePlay.View.Grounds;
using Game.State.GameResources;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.Root.View
{
    public class WorldGameplayRootViewModel
    {
        private readonly ResourcesService _resourcesService;
        public readonly IObservableCollection<BuildingViewModel> AllBuildings;
        public readonly IObservableCollection<GroundViewModel> AllGrounds;
 
        public WorldGameplayRootViewModel(
            BuildingsService buildingsService,
            GroundsService groundsService,
            ResourcesService resourcesService)
        {
            _resourcesService = resourcesService;
            
            AllBuildings = buildingsService.AllBuildings;
            AllGrounds = groundsService.AllGrounds;

            resourcesService.ObservableResource(ResourceType.SoftCurrency).Subscribe(
                newValue => Debug.Log($"SoftCurrency = {newValue}"));
            
            resourcesService.ObservableResource(ResourceType.HardCurrency).Subscribe(
                newValue => Debug.Log($"HardCurrency = {newValue}"));
        }

    }
}