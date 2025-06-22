using System;
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

        private readonly FrameService _frameService;

        private readonly RoadsService _roadsService;
        // private readonly DIContainer _container;

        //   public readonly IObservableCollection<RoadViewModel> AllRoads;
        public readonly IObservableCollection<TowerViewModel> AllTowers;
        public readonly IObservableCollection<GroundViewModel> AllGrounds;
        public readonly IObservableCollection<RoadViewModel> AllRoads;

        public readonly IObservableCollection<FrameBlock> FramesBlock;
        public CastleViewModel CastleViewModel { get; private set; }

        public WorldGameplayRootViewModel(
            //  RoadService roadsService,
            GroundsService groundsService,
            TowersService towersService,
            CastleService castleService,
            FsmGameplay fsmGameplay,
            FrameService frameService,
            PlacementService placementService,
            RoadsService roadsService
            //DIContainer container
        )
        {
            _fsmGameplay = fsmGameplay;
            _frameService = frameService;
            _roadsService = roadsService;
            //_container = container;

            AllRoads = roadsService.AllRoads;
            AllGrounds = groundsService.AllGrounds;
            AllTowers = towersService.AllTowers;
            FramesBlock = frameService.FramesBlock;
            CastleViewModel = castleService.CastleViewModel;

            //Изменение состояние Геймплея
            _fsmGameplay.Fsm.StateCurrent.Subscribe(newState =>
            {
                if (newState.GetType() == typeof(FsmStateBuild))
                {
                    var reward = ((FsmStateBuild)newState).GetRewardCard();
                    if (reward.RewardType == RewardType.Tower)
                    {
                        var position = placementService.GetNewPositionTower();
                        var level = towersService.Levels[reward.ConfigId];
                        //towersService.PlaceTower(reward.ConfigId, position);
                        frameService.CreateFrameTower(position, level, reward.ConfigId);
                    }
                }

                if (newState.GetType() == typeof(FsmStateBuildEnd))
                {
                    var card = ((FsmStateBuildEnd)newState).GetRewardCard();
                    var position = _fsmGameplay.Fsm.Position.CurrentValue; 
                    
                    switch (card.RewardType)
                    {
                        case RewardType.Tower: 
                            towersService.PlaceTower(card.ConfigId, position); 
                            frameService.RemoveFrame();
                            break; 
                        case RewardType.Ground: 
                            groundsService.PlaceGround(position);
                            frameService.RemoveFrame();
                            break;
                        case RewardType.Road: 
                            Debug.Log("Размещение дороги. В разработке"); 
                            frameService.RemoveFrame();
                            break;
                        case RewardType.TowerBust: towersService.ApplyBust(card.TargetId, card.ConfigId); break;
                        case RewardType.TowerMove: towersService.MoveTower(card.UniqueId, position); break;
                        case RewardType.TowerReplace: towersService.ReplaceTower(card.UniqueId, card.UniqueId2); break;
                        case RewardType.SkillBust: Debug.Log("Усиление навыка. В разработке"); break;
                        case RewardType.HeroBust: Debug.Log("Усиление героя. В разработке"); break;
                        default: throw new Exception($"Неверный тип награды {card.RewardType}"); 
                    }
                }

                if (newState.GetType() == typeof(FsmStateBuildBegin))
                {
                    if (newState.Fsm.PreviousState.GetType() == typeof(FsmStateBuild)) //Возврат от режима строим
                    {
                        frameService.RemoveFrame();
                        //Удаляем фреймы
                    }
                }

//                Debug.Log("newState = " + JsonConvert.SerializeObject(newState.Params, Formatting.Indented));
            });

            _fsmGameplay.Fsm.Position.Subscribe(newPosition =>
            {
//                Debug.Log("newPosition " + newPosition.x + " / " + newPosition.y);
                if (_fsmGameplay.IsStateBuild())
                {
                    frameService.MoveFrame(newPosition);
                  /*  
                    var card = ((FsmStateBuild)_fsmGameplay.Fsm.StateCurrent.Value).GetRewardCard();
                    if (card.RewardType == RewardType.Tower)
                    {
                        foreach (TowerViewModel towerViewModel in AllTowers)
                        {
                            if (towerViewModel.IsFrame.CurrentValue) towersService.MoveTower(towerViewModel.TowerEntityId, newPosition);
                        }
                    }*/
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

        //Если при нажатии клавиши, под ним фрейм, то выделяем его и возвращаем true
        public bool DownFrame(Vector2 position)
        {
//            Debug.Log(" Нажали на фрейм ");
            var vectorInt = new Vector2Int(
                Mathf.FloorToInt(position.x + 0.5f),
                Mathf.FloorToInt(position.y + 0.5f)
            );
            var result = _frameService.IsPosition(vectorInt);
      //      Debug.Log(" Нашли фрейм " + result);

            if (result) _frameService.SelectedFrame();
            return result;
        }

        public void UpFrame()
        {
       //     Debug.Log(" Отпустили фрейм ");
            _frameService.UnSelectedFrame();
        }

        public void MoveFrame(Vector2 position)
        {
//            Debug.Log(" Двигаем фрейм ");
            var vectorInt = new Vector2Int(
                Mathf.FloorToInt(position.x + 0.5f),
                Mathf.FloorToInt(position.y + 0.5f)
            );
            _frameService.MoveFrame(vectorInt);
        }
    }
}