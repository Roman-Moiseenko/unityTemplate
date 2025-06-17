using System.Collections.ObjectModel;
using DI;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.States;
using Game.GamePlay.Services;
using Game.GamePlay.View.Buildings;
using Game.GamePlay.View.Castle;
using Game.GamePlay.View.Grounds;
using Game.GamePlay.View.Roads;
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
        private readonly FsmGameplay _fsmGameplay;
       // private readonly DIContainer _container;

     //   public readonly IObservableCollection<RoadViewModel> AllRoads;
        public readonly IObservableCollection<TowerViewModel> AllTowers;
        public readonly IObservableCollection<GroundViewModel> AllGrounds;
        public CastleViewModel CastleViewModel { get; private set; }
 
        public WorldGameplayRootViewModel(
          //  RoadService roadsService,
            GroundsService groundsService,
            TowersService towersService,
            CastleService castleService,
            FsmGameplay fsmGameplay
         //DIContainer container
            )
        {
            _fsmGameplay = fsmGameplay;
            //_container = container;

           // AllRoads = roadsService.AllRoads;
            AllGrounds = groundsService.AllGrounds;
            AllTowers = towersService.AllTowers;
            CastleViewModel = castleService.CastleViewModel;
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

        public void ClickEntity(Vector2 position)
        {
            if (_fsmGameplay.IsStateGaming() || _fsmGameplay.IsStateBuildBegin())
            {
                //TODO проверяем, чтоб показать Info()
                foreach (var towerViewModel in AllTowers)
                {
                    if (towerViewModel.IsPosition(position))
                    {
                        Debug.Log(" Это башня " + towerViewModel.ConfigId);
                        return;
                    }
                }

                
                if (CastleViewModel.IsPosition(position))
                    Debug.Log(" Это крепость " + CastleViewModel.ConfigId);
            }
        }
    }
}