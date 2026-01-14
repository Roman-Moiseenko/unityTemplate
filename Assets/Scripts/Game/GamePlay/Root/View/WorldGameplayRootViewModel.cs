using System;
using System.Collections.ObjectModel;
using DI;
using Game.Common;
using Game.GamePlay.Classes;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.GameplayStates;
using Game.GamePlay.Services;
using Game.GamePlay.View.AttackAreas;
using Game.GamePlay.View.Castle;
using Game.GamePlay.View.Frames;
using Game.GamePlay.View.Grounds;
using Game.GamePlay.View.Map;
using Game.GamePlay.View.Mobs;
using Game.GamePlay.View.Roads;
using Game.GamePlay.View.Towers;
using Game.GamePlay.View.Waves;
using Game.State.Gameplay;
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
        private readonly WaveService _waveService;
        private readonly GameplayCamera _cameraService;
        private readonly DamageService _damageService;
        private readonly Subject<Unit> _entityClick;
        private readonly Subject<TowerViewModel> _towerClick;
        // private readonly DIContainer _container;

        //   public readonly IObservableCollection<RoadViewModel> AllRoads;
        public readonly IObservableCollection<TowerViewModel> AllTowers;
        public readonly IObservableCollection<MobViewModel> AllMobs;
        public readonly IObservableCollection<GroundViewModel> AllGrounds;
        public readonly IObservableCollection<BoardViewModel> AllBoards;
        public readonly IObservableCollection<RoadViewModel> AllRoads;

        public readonly IObservableCollection<FrameBlockViewModel> FrameBlockViewModels;
        public CastleViewModel CastleViewModel { get; private set; }
        public GateWaveViewModel GateWaveViewModel { get; private set; }
        public GateWaveViewModel GateWaveViewModelSecond { get; private set; }
        public AttackAreaViewModel AreaViewModel { get; }
        
        public MapFogViewModel MapFogViewModel { get;  }
       // public DamagePopupViewModel DamagePopupViewModel { get; private set; }
        
        public ReactiveProperty<Vector2Int> CameraMove;

        private bool _isFrameDownClick = false; //Отслеживаем что перетаскивать Фрейм ил Камеру
        
        public WorldGameplayRootViewModel(
            //  RoadService roadsService,
            GroundsService groundsService,
            TowersService towersService,
            CastleService castleService,
            FsmGameplay fsmGameplay,
            FrameService frameService,
            PlacementService placementService,
            RoadsService roadsService,
            WaveService waveService,
            GameplayCamera cameraService,
            DamageService damageService,
            //ShotService shotService,
            DIContainer container
        )
        {
            _fsmGameplay = fsmGameplay;
            _frameService = frameService;
            _roadsService = roadsService;
            _waveService = waveService;
            _cameraService = cameraService;
            _damageService = damageService;
           // _shotService = shotService;
            //Регистрируем события 
            _entityClick = new Subject<Unit>(); //клик по объектам игрового мира, не UI 
            container.RegisterInstance(AppConstants.CLICK_WORLD_ENTITY, _entityClick);
            _towerClick = new Subject<TowerViewModel>();
            container.RegisterInstance(_towerClick);
            
            //_container = container;

            AllRoads = roadsService.AllRoads;
            AllGrounds = groundsService.AllGrounds;
            AllBoards = groundsService.AllBoards;
            AllTowers = towersService.AllTowers;
            AllMobs = waveService.AllMobsOnWay;
            //AllShots = shotService.AllShots;
            FrameBlockViewModels = frameService.ViewModels;
            CastleViewModel = castleService.CastleViewModel;
            GateWaveViewModel = waveService.GateWaveViewModel;
            GateWaveViewModelSecond = waveService.GateWaveViewModelSecond;

            AreaViewModel = new AttackAreaViewModel(_towerClick);
            MapFogViewModel = new MapFogViewModel(groundsService);
            //DamagePopupViewModel = new DamagePopupViewModel(damageService.AllDamages);
            
            CameraMove = new ReactiveProperty<Vector2Int>(new Vector2Int(0, 0));
            CameraMove.Subscribe(p => _cameraService.MoveCamera(p));
            //Изменение состояние Геймплея
            _fsmGameplay.Fsm.StateCurrent.Subscribe(newState =>
            {
                //Режим строительства
                if (newState.GetType() == typeof(FsmStateBuild))
                {
                    CameraMove.Value = Vector2Int.zero;
                    var reward = ((FsmStateBuild)newState).GetRewardCard();
                    Vector2Int position = new Vector2Int();
                    if (reward.RewardType == RewardType.Tower)
                    {
                        position = placementService.GetNewPositionTower(reward.OnRoad);
                        var level = towersService.Levels[reward.ConfigId];
                        frameService.CreateFrameTower(position, level, reward.ConfigId, AreaViewModel);
                    }

                    if (reward.RewardType == RewardType.Road)
                    {
                        position = placementService.GetNewPositionRoad();
                        frameService.CreateFrameRoad(position, reward.ConfigId, placementService.GetNewDirectionRoad());
                    }
                    if (reward.RewardType == RewardType.Ground)
                    {
                        position = placementService.GetNewPositionGround();
                        frameService.CreateFrameGround(position);
                    }
                    _fsmGameplay.SetPosition(position); //Сохраняем позицию сущности в состоянии
                    CameraMove.Value = position; //центрируем карту
                    
                }
                //Режим завершения строительства
                if (newState.GetType() == typeof(FsmStateBuildEnd))
                {
                    
                    var card = ((FsmStateBuildEnd)newState).GetRewardCard();
                    var position = _fsmGameplay.GetPosition(); 
                    //Debug.Log("WorldGameplayRootViewModel Режим конца строительства + " + card.RewardType);
                    switch (card.RewardType)
                    {
                        case RewardType.Tower: 
                            //Как только завершится удаления фрейма, размещаем элементы
                            frameService.RemoveFrameAnimation().Where(x => x).Subscribe(_ =>
                            {
                                towersService.PlaceTower(card.ConfigId, position); 
                            });
                            break; 
                        case RewardType.Ground:
                            frameService.RemoveFrameAnimation().Where(x => x).Subscribe(_ =>
                            {
                                foreach (var groundPosition in frameService.GetGrounds())
                                {
                                    groundsService.PlaceGround(groundPosition);
                                }
                                groundsService.PlaceGround(position);
                            });
                            break;
                        case RewardType.Road:
                            frameService.RemoveFrameAnimation().Where(x => x).Subscribe(_ =>
                            {
                                var isMainPath = frameService.IsMainPath();
                                foreach (var road in frameService.GetRoadsForBuild())
                                {
                                    roadsService.PlaceRoad(road.Position, road.IsTurn, road.Rotate, isMainPath);
                                    groundsService.PlaceGround(road.Position);
                                }
                            });

                            //frameService.RemoveFrame();
                            
                            break;
                        case RewardType.TowerLevelUp: towersService.LevelUpTower(card.ConfigId); break;
                        case RewardType.TowerMove: towersService.MoveTower(card.UniqueId, position); break;
                        case RewardType.TowerReplace: towersService.ReplaceTower(card.UniqueId, card.UniqueId2); break;
                        case RewardType.SkillLevelUp: Debug.Log("Усиление навыка. В разработке"); break;
                        case RewardType.HeroLevelUp: Debug.Log("Усиление героя. В разработке"); break;
                        default: throw new Exception($"Неверный тип награды {card.RewardType}"); 
                    }
                }

                //Режим начала строительства/выбора наград
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
                if (_fsmGameplay.IsStateBuild())
                {
                    frameService.MoveFrame(newPosition); //Переносим объект
                }
                if (_fsmGameplay.IsStateGaming())
                {
                    _cameraService.MoveCamera(Vector2Int.zero);   //Центрируем карту
                }
            });
/*
            resourcesService.ObservableResource(ResourceType.SoftCurrency).Subscribe(
                newValue => Debug.Log($"SoftCurrency = {newValue}"));

            resourcesService.ObservableResource(ResourceType.HardCurrency).Subscribe(
                newValue => Debug.Log($"HardCurrency = {newValue}"));
            */
        }

        public void ClickEntity(Vector2 mousePosition)
        {
//            Debug.Log(" ClickEntity "+ mousePosition);

            var position = _cameraService.GetWorldPoint(mousePosition);
            _entityClick.OnNext(Unit.Default);
            if (_fsmGameplay.IsStateGaming() || _fsmGameplay.IsStateBuildBegin())
            {
                foreach (var towerViewModel in AllTowers)
                {
                    if (towerViewModel.IsPosition(position))
                    {
                        _towerClick.OnNext(towerViewModel);
                        _cameraService.MoveCamera(towerViewModel.Position.Value);
                        return;
                    }
                }


                if (CastleViewModel.IsPosition(position))
                    Debug.Log(" Это крепость " + CastleViewModel.ConfigId);
                AreaViewModel.Hide();
                return;
            }
            
            if (_fsmGameplay.IsStateBuild())
            {
                _fsmGameplay.SetPosition(new Vector2Int(
                    Mathf.FloorToInt(position.x + 0.5f), 
                    Mathf.FloorToInt(position.y + 0.5f)
                    ));   
                //_fsmGameplay.Fsm.Position.Value = new Vector2Int(Mathf.FloorToInt(position.x + 0.5f), Mathf.FloorToInt(position.y + 0.5f));
                
                var card = (RewardCardData)(_fsmGameplay.Fsm.StateCurrent.Value.Params);
                card.Position.x = Mathf.FloorToInt(position.x + 0.5f);
                card.Position.y = Mathf.FloorToInt(position.y + 0.5f);
                _fsmGameplay.Fsm.StateCurrent.Value.Params = card;
                
            }
        }

        //Если при нажатии клавиши, под ним фрейм, то выделяем его и возвращаем true
        public void StartMoving(Vector2 mousePosition)
        {
            var position = _cameraService.GetWorldPoint(mousePosition);
                
            var vectorInt = new Vector2Int(
                Mathf.FloorToInt(position.x + 0.5f),
                Mathf.FloorToInt(position.y + 0.5f)
            );
            //TODO Проверить при повторном перемещении
            _isFrameDownClick = _frameService.IsPosition(vectorInt);
            if (_isFrameDownClick)
            {
                _frameService.SelectedFrame();
            }
            else
            {
                //Debug.Log(" StartMoving "+ mousePosition);
                _cameraService.OnPointDown(mousePosition);
            }
        }
        
        public void StartGameplayServices()
        {
            _waveService.StartNextWave();
        }

        public void Update()
        {
            _cameraService?.UpdateMoving(); //Движение камеры
            _cameraService?.AutoMoving();
            _damageService.Update();
        }
        
        public void FinishMoving(Vector2 mousePosition)
        {
            if (_isFrameDownClick)
            {
                _frameService.UnSelectedFrame(); //Завершаем движение фрейма //    Отпустили фрейм 
            }
            else
            {
              //  Debug.Log(" FinishMoving "+ mousePosition);

                _cameraService.OnPointUp(mousePosition);//Завершаем движение камеры
            }
        }

        public void ProcessMoving(Vector2 mousePosition)
        {
            if (_isFrameDownClick) //Двигаем фрейм или показываем инфо 
            {
                ClickEntity(mousePosition);
            }
            else //Двигаем камеру
            {
//                Debug.Log(" ProcessMoving "+ mousePosition);

                _cameraService.OnPointMove(mousePosition);
            }
        }
    }
}