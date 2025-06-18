using System.Collections.ObjectModel;
using DI;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.States;
using Game.GamePlay.Services;
using Game.GamePlay.View.Buildings;
using Game.GamePlay.View.Castle;
using Game.GamePlay.View.Frames;
using Game.GamePlay.View.Grounds;
using Game.GamePlay.View.Roads;
using Game.GamePlay.View.Towers;
using Game.MainMenu.Services;
using Game.State.Gameplay;
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
        public readonly IObservableCollection<FrameViewModel> AllFrames;
        public CastleViewModel CastleViewModel { get; private set; }

        public WorldGameplayRootViewModel(
            //  RoadService roadsService,
            GroundsService groundsService,
            TowersService towersService,
            CastleService castleService,
            FsmGameplay fsmGameplay,
            FrameService frameService
            //DIContainer container
        )
        {
            _fsmGameplay = fsmGameplay;
            //_container = container;

            // AllRoads = roadsService.AllRoads;
            AllGrounds = groundsService.AllGrounds;
            AllTowers = towersService.AllTowers;
            AllFrames = frameService.AllFrames;
            CastleViewModel = castleService.CastleViewModel;

            //Изменение состояние Геймплея
            _fsmGameplay.Fsm.StateCurrent.Subscribe(newState =>
            {
                if (newState.GetType() == typeof(FsmStateBuild))
                {
                    var reward = ((FsmStateBuild)newState).GetRewardCard();
                    if (reward.RewardType == RewardType.Tower)
                    {
                        var position = new Vector2Int(Random.Range(-1, 5), Random.Range(-1, 3));
                        towersService.PlaceTower(reward.ConfigId, position);
                    }
                }

                if (newState.GetType() == typeof(FsmStateBuildEnd))
                {
                    //Удаляем фреймы
                    foreach (var towerViewModel in AllTowers)
                    {
                        if (towerViewModel.IsFrame.CurrentValue)
                        {
                            towerViewModel.IsFrame.Value = false;
                           // return;
                        }
                    }
                    
                }

                if (newState.GetType() == typeof(FsmStateBuildBegin))
                {
                    if (newState.Fsm.PreviousState.GetType() == typeof(FsmStateBuild)) //Возврат от режима строим
                    {
                        foreach (var towerViewModel in AllTowers)
                        {
                            if (towerViewModel.IsFrame.CurrentValue)
                            {
                                towersService.DeleteTower(towerViewModel.TowerEntityId);
                                return;
                            }
                        }
                      /*  foreach (var groundViewModel in AllGrounds)
                        {
                            if (groundViewModel.IsFrame.CurrentValue) groundViewModel.DeleteGround(groundViewModel.TowerEntityId);
                        } 
                        foreach (var roadViewModel in AllRoads)
                        {
                            if (roadViewModel.IsFrame.CurrentValue) roadsService.DeleteRoad(roadViewModel.TowerEntityId);
                        }
                        */
                        //Удаляем фреймы
                    }
                }

                Debug.Log("newState = " + JsonConvert.SerializeObject(newState.Params, Formatting.Indented));
            });

            _fsmGameplay.Fsm.Position.Subscribe(newPosition =>
            {
//                Debug.Log("newPosition " + newPosition.x + " / " + newPosition.y);
                if (_fsmGameplay.IsStateBuild())
                {
                    var card = ((FsmStateBuild)_fsmGameplay.Fsm.StateCurrent.Value).GetRewardCard();
                    if (card.RewardType == RewardType.Tower)
                    {
                        foreach (TowerViewModel towerViewModel in AllTowers)
                        {
                            if (towerViewModel.IsFrame.CurrentValue) towersService.MoveTower(towerViewModel.TowerEntityId, newPosition);
                        }
                    }
                    //Переносим объект
                }

                if (_fsmGameplay.IsStateGaming())
                {
                    //Центрируем карту ??
                }
            });
/*
            resourcesService.ObservableResource(ResourceType.SoftCurrency).Subscribe(
                newValue => Debug.Log($"SoftCurrency = {newValue}"));

            resourcesService.ObservableResource(ResourceType.HardCurrency).Subscribe(
                newValue => Debug.Log($"HardCurrency = {newValue}"));
            */
        }


        public void ControlInput(Vector3Int position, string ConfigId)
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

                return;
            }

            if (_fsmGameplay.IsStateBuild())
            {
//                Debug.Log("position = " + JsonUtility.ToJson(position));
                _fsmGameplay.Fsm.Position.Value =
                    new Vector2Int(Mathf.FloorToInt(position.x + 0.5f), Mathf.FloorToInt(position.y + 0.5f));
                
                var card = (RewardCardData)(_fsmGameplay.Fsm.StateCurrent.Value.Params);
                card.Position.x = Mathf.FloorToInt(position.x + 0.5f);
                card.Position.y = Mathf.FloorToInt(position.y + 0.5f);
                _fsmGameplay.Fsm.StateCurrent.Value.Params = card;
                
            }
        }
    }
}