using Game.GamePlay.Services;
using Game.GamePlay.View.Buildings;
using Game.GamePlay.View.Grounds;
using Game.GamePlay.View.Towers;
using Game.MainMenu.Services;
using Game.State.GameResources;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.Root.View
{
    public class WorldGameplayRootViewModel
    {
        public readonly IObservableCollection<BuildingViewModel> AllBuildings;
        public readonly IObservableCollection<TowerViewModel> AllTowers;
        public readonly IObservableCollection<GroundViewModel> AllGrounds;
 
        public WorldGameplayRootViewModel(
            BuildingsService buildingsService,
            GroundsService groundsService,
            TowersService towersService
            )
        {
            
            AllBuildings = buildingsService.AllBuildings;
            AllGrounds = groundsService.AllGrounds;
            AllTowers = towersService.AllTowers;
/*
            resourcesService.ObservableResource(ResourceType.SoftCurrency).Subscribe(
                newValue => Debug.Log($"SoftCurrency = {newValue}"));
            
            resourcesService.ObservableResource(ResourceType.HardCurrency).Subscribe(
                newValue => Debug.Log($"HardCurrency = {newValue}"));
            */
        }


        public void ControlInput(Vector3Int position, string ConfigId )
        {
            
        }

    }
}