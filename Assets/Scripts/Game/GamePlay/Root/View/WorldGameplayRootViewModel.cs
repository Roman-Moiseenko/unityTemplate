using System;
using System.Collections;
using DI;
using Game.Common;
using Game.GamePlay.Classes;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.GameplayStates;
using Game.GamePlay.Services;
using Game.GamePlay.View.Castle;
using Game.GamePlay.View.Frames;
using Game.GamePlay.View.Grounds;
using Game.GamePlay.View.Map;
using Game.GamePlay.View.Mobs;
using Game.GamePlay.View.Roads;
using Game.GamePlay.View.Towers;
using Game.GamePlay.View.Warriors;
using Game.GamePlay.View.Waves;
using Game.State.Gameplay;
using ObservableCollections;
using R3;
using Scripts.Utils;
using UnityEngine;

namespace Game.GamePlay.Root.View
{
    public class WorldGameplayRootViewModel
    {
        public readonly IObservableCollection<TowerViewModel> AllTowers;
        public readonly IObservableCollection<WarriorViewModel> AllWarriors;
        public readonly IObservableCollection<MobViewModel> AllMobs;
        public readonly IObservableCollection<GroundViewModel> AllGrounds;
        public readonly IObservableCollection<BoardViewModel> AllBoards;
        public readonly IObservableCollection<RoadViewModel> AllRoads;
        public readonly IObservableCollection<FrameBlockViewModel> FrameBlockViewModels;
        public CastleViewModel CastleViewModel { get; private set; }
        public GateWaveViewModel GateWaveViewModel { get; private set; }
        public GateWaveViewModel GateWaveViewModelSecond { get; private set; }
        //public AttackAreaViewModel AreaViewModel { get; }
        public MapFogViewModel MapFogViewModel { get; }

        private readonly FsmGameplay _fsmGameplay;
        private readonly FrameService _frameService;
        private readonly WaveService _waveService;
        private readonly GameplayCamera _cameraService;
        private readonly DamageService _damageService;
        public readonly Subject<Unit> EntityClick;
        public readonly Subject<TowerViewModel> TowerClick;
        private bool _isFrameDownClick; //Отслеживаем что перетаскивать Фрейм ил Камеру
        private readonly Coroutines _coroutines;

        public WorldGameplayRootViewModel(
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
            WarriorService warriorService,
            DIContainer container
        )
        {
            _coroutines = GameObject.Find(AppConstants.COROUTINES).GetComponent<Coroutines>();
            
            _fsmGameplay = fsmGameplay;
            _frameService = frameService;
            _waveService = waveService;
            _cameraService = cameraService;
            _damageService = damageService;
            //Регистрируем события в контейнер, для вытаскивания в UI
            EntityClick = new Subject<Unit>(); //клик по объектам игрового мира, не UI 
            container.RegisterInstance(AppConstants.CLICK_WORLD_ENTITY, EntityClick);
            TowerClick = new Subject<TowerViewModel>();
            container.RegisterInstance(TowerClick);
            //TODO Клик на 

            AllRoads = roadsService.AllRoads;
            AllGrounds = groundsService.AllGrounds;
            AllBoards = groundsService.AllBoards;
            AllTowers = towersService.AllTowers;
            AllMobs = waveService.AllMobsOnWay;
            AllWarriors = warriorService.AllWarriors;

            FrameBlockViewModels = frameService.ViewModels;
            CastleViewModel = castleService.CastleViewModel;
            GateWaveViewModel = waveService.GateWaveViewModel;
            GateWaveViewModelSecond = waveService.GateWaveViewModelSecond;

            //AreaViewModel = new AttackAreaViewModel();
            MapFogViewModel = new MapFogViewModel(groundsService);

            //Изменение состояние Геймплея
            _fsmGameplay.Fsm.StateCurrent.Subscribe(newState =>
            {
                //Режим строительства
                if (newState.GetType() == typeof(FsmStateBuild))
                {
                    var reward = ((FsmStateBuild)newState).GetRewardCard();
                    var position = Vector2Int.zero;
                    if (reward.RewardType == RewardType.Tower)
                    {
                        position = placementService.GetNewPositionTower(reward.OnRoad);
                        var level = towersService.Levels[reward.ConfigId];
                        EntityClick.OnNext(Unit.Default);
                        frameService.CreateFrameTower(position, level, reward.ConfigId);
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
                    _cameraService.MoveCamera(position); //центрируем карту на объект
                }

                //Режим завершения строительства
                if (newState.GetType() == typeof(FsmStateBuildEnd))
                {
                    var card = ((FsmStateBuildEnd)newState).GetRewardCard();
                    var position = _fsmGameplay.GetPosition();

                    switch (card.RewardType)
                    {
                        case RewardType.Tower:
                            //Как только завершится удаления фрейма, размещаем элементы
                            frameService.RemoveFrameAnimation().Where(x => x).Subscribe(_ =>
                            {
                                towersService.PlaceTower(card.ConfigId, position);
                                _fsmGameplay.Fsm.SetState<FsmStateGamePlay>();
                            });
                            break;
                        case RewardType.Ground:
                            //Без пыли
                            frameService.RemoveFrame();
                            //frameService.RemoveFrameAnimation().Where(x => x).Subscribe(_ =>
                            //{
                            foreach (var groundPosition in frameService.GetGrounds())
                            {
                                groundsService.PlaceGround(groundPosition);
                            }

                            groundsService.PlaceGround(position);
                            _fsmGameplay.Fsm.SetState<FsmStateGamePlay>();

                            //});
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

                                _fsmGameplay.Fsm.SetState<FsmStateGamePlay>();
                            });
                            break;
                        case RewardType.TowerLevelUp:
                            towersService.LevelUpTower(card.ConfigId);
                            //TODO Переделать на дождаться завершения анимации Либо подписка на Subject()
                            _coroutines.StartCoroutine(StartWaitBeforeGameplay(0.5f));
                            //_fsmGameplay.Fsm.SetState<FsmStateGamePlay>();
                            break;
                        case RewardType.TowerMove:
                            towersService.MoveTower(card.UniqueId, position);
                            _fsmGameplay.Fsm.SetState<FsmStateGamePlay>();
                            break;
                        case RewardType.TowerReplace:
                            towersService.ReplaceTower(card.UniqueId, card.UniqueId2);
                            _fsmGameplay.Fsm.SetState<FsmStateGamePlay>();
                            break;
                        case RewardType.SkillLevelUp:
                            Debug.Log("Усиление навыка. В разработке");
                            _fsmGameplay.Fsm.SetState<FsmStateGamePlay>();
                            break;
                        case RewardType.HeroLevelUp:
                            Debug.Log("Усиление героя. В разработке");
                            _fsmGameplay.Fsm.SetState<FsmStateGamePlay>();
                            break;
                        default: throw new Exception($"Неверный тип награды {card.RewardType}");
                    }
                }

                //Режим начала строительства/выбора наград
                if (newState.GetType() == typeof(FsmStateBuildBegin))
                {
                    if (newState.Fsm.PreviousState.GetType() == typeof(FsmStateBuild)) //Возврат от режима строим
                        frameService.RemoveFrame();
                    //Удаляем фреймы
                }
                
            });

            _fsmGameplay.Fsm.Position.Subscribe(newPosition =>
            {
                if (_fsmGameplay.IsStateBuild())
                {
                    frameService.MoveFrame(newPosition); //Переносим объект
                }

                if (_fsmGameplay.IsStateGaming())
                {
                    _cameraService.MoveCamera(Vector2Int.zero); //Центрируем карту
                }
            });
        }
        
        public void ClickEntity(Vector2 mousePosition)
        {
            var position = _cameraService.GetWorldPoint(mousePosition);

            //TODO получить объект на который кликнули
            
            if (_fsmGameplay.IsStateGaming() || _fsmGameplay.IsStateBuildBegin())
            {
                foreach (var towerViewModel in AllTowers)
                {
                    if (towerViewModel.IsPosition(position))
                    {
                        TowerClick.OnNext(towerViewModel);
                        _cameraService.MoveCamera(towerViewModel.Position.Value);
                        return;
                    }
                }

                if (CastleViewModel.IsPosition(position))
                    Debug.Log(" Это крепость " + CastleViewModel.ConfigId);
                //AreaViewModel.Hide();
                EntityClick.OnNext(Unit.Default);
                return;
            }

            if (_fsmGameplay.IsStateBuild())
            {
                _fsmGameplay.SetPosition(new Vector2Int(
                    Mathf.FloorToInt(position.x + 0.5f),
                    Mathf.FloorToInt(position.y + 0.5f)
                ));
                var card = (RewardCardData)(_fsmGameplay.Fsm.StateCurrent.Value.Params);
                card.Position.x = Mathf.FloorToInt(position.x + 0.5f);
                card.Position.y = Mathf.FloorToInt(position.y + 0.5f);
                _fsmGameplay.Fsm.StateCurrent.Value.Params = card;
            }

            EntityClick.OnNext(Unit.Default);
        }

        //Если при нажатии клавиши, под ним фрейм, то выделяем его и возвращаем true
        public void StartMoving(Vector2 mousePosition)
        {
            var position = _cameraService.GetWorldPoint(mousePosition);

            var vectorInt = new Vector2Int(
                Mathf.FloorToInt(position.x + 0.5f),
                Mathf.FloorToInt(position.y + 0.5f)
            );
            _isFrameDownClick = _frameService.IsPosition(vectorInt);
//            Debug.Log("StartMoving " + _isFrameDownClick);
            if (_isFrameDownClick)
            {
                _frameService.SelectedFrame();
            }
            else
            {
                _cameraService.OnPointDown(mousePosition);
            }
        }

        public void ScalingCamera(bool scalingUp)
        {
            _cameraService.ScalingCamera(scalingUp);
        }

        public void StartGameplayServices()
        {
            _waveService.StartNextWave();
        }

        public void Update()
        {
            _cameraService?.UpdateMoving(); //Движение камеры
            //_cameraService?.AutoMoving();
            //_damageService.Update();
        }

        public void FinishMoving(Vector2 mousePosition)
        {
            if (_isFrameDownClick)
            {
                _frameService.UnSelectedFrame(); //Завершаем движение фрейма //    Отпустили фрейм 
            }
            else
            {
                _cameraService.OnPointUp(mousePosition); //Завершаем движение камеры
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
                _cameraService.OnPointMove(mousePosition);
            }
        }

        private IEnumerator StartWaitBeforeGameplay(float delta)
        {
            yield return new WaitForSecondsRealtime(delta);
            _fsmGameplay.Fsm.SetState<FsmStateGamePlay>();
        }
        
        
    }
}