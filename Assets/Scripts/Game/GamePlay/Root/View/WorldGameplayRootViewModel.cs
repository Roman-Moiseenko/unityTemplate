using System;
using System.Collections;
using DI;
using Game.Common;
using Game.GamePlay.Classes;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.GameplayStates;
using Game.GamePlay.Fsm.TowerStates;
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
        public ReactiveProperty<FramePlacementViewModel> FramePlacement = new(null);
        public CastleViewModel CastleViewModel { get; private set; }
        public GateWaveViewModel GateWaveViewModel { get; private set; }
        public GateWaveViewModel GateWaveViewModelSecond { get; private set; }
        //public AttackAreaViewModel AreaViewModel { get; }
        public MapFogViewModel MapFogViewModel { get; }

        private readonly FsmGameplay _fsmGameplay;
        private readonly FsmTower _fsmTower;
        private readonly FrameService _frameService;
        private readonly WaveService _waveService;
        private readonly GameplayCamera _cameraService;
        private readonly DamageService _damageService;
        //Публичны, для передачи из RootBinder в те Binder, где модели необходимо реагировать на события клик
        private readonly Subject<Unit> _entityClick;
        private readonly Subject<TowerViewModel> _towerClick;
        private bool _isFrameDownClick; //Отслеживаем что перетаскивать Фрейм ил Камеру
      //  private readonly Coroutines _coroutines;

        public WorldGameplayRootViewModel(
            GroundsService groundsService,
            TowersService towersService,
            CastleService castleService,
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
            _fsmGameplay = container.Resolve<FsmGameplay>();
            _fsmTower = container.Resolve<FsmTower>();
            _frameService = frameService;
            _waveService = waveService;
            _cameraService = cameraService;
            _damageService = damageService;
            //клики по объектам игрового мира, не UI 
            _entityClick = container.Resolve<Subject<Unit>>(AppConstants.CLICK_WORLD_ENTITY);
            _towerClick = container.Resolve<Subject<TowerViewModel>>();

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
                        _entityClick.OnNext(Unit.Default);
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
                            towersService.LevelUpTower(card.ConfigId)
                                .Where(x => x)
                                .Subscribe(_ => _fsmGameplay.Fsm.SetState<FsmStateGamePlay>());
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

                if (_fsmTower.IsPlacement())
                {
                    var tower = _fsmTower.GetTowerViewModel();
                    tower.Placement.OnNext(newPosition);
                }
            });

            _fsmTower.Fsm.StateCurrent.Subscribe(newState =>
            {
                if (newState.GetType() == typeof(FsmTowerPlacement))
                {
                    //Создать FramePlacement
                    var frame = new FramePlacementViewModel(_fsmTower.GetTowerViewModel(), placementService);
                    FramePlacement.Value = frame;
                }
                else
                {
                    if (newState.GetType() == typeof(FsmTowerPlacementEnd))
                    { 
                        //Сохраняем новое значение
                        towersService.SetPlacement();
                    }

                    if (newState.GetType() == typeof(FsmTowerSelected) && _fsmTower.Fsm.PreviousState.GetType() == typeof(FsmTowerPlacement))
                    {
                        //Возвращаем старое значение
                        towersService.ResumePlacement(FramePlacement.CurrentValue.TowerUniqueId, FramePlacement.CurrentValue.StartPosition);
                    }
                    
                    //Удалить FramePlacement, если был создан
                    FramePlacement.Value = null;
                }
                
            });
        }
        
        public void ClickEntity(Vector2 mousePosition)
        {
            var position = _cameraService.GetWorldPoint(mousePosition);

            //TODO получить объект на который кликнули
            
            //Если Игра или Начало строительства
            //В этих же режим проходят все состояния FsmTower 
            if (_fsmGameplay.IsStateGaming() || _fsmGameplay.IsStateBuildBegin())
            {
                //И с башней нет работы или выделена (для смены на другую)
                if (_fsmTower.IsNone() || _fsmTower.IsSelected())
                {
                    foreach (var towerViewModel in AllTowers)
                    {
                        //Кликнули по башне
                        if (towerViewModel.IsPosition(position))
                        {
                            _towerClick.OnNext(towerViewModel); //TODO Удалить
                            
                            if (_fsmTower.IsSelected()) _fsmTower.Fsm.SetState<FsmTowerNone>(); //Сбрасываем выделение.
                            _fsmTower.Fsm.SetState<FsmTowerSelected>(towerViewModel); //Башня выделена
                            _cameraService.MoveCamera(towerViewModel.Position.Value);
                            return;
                        }
                    }
                }

                if (_fsmTower.IsPlacement())
                {
                    _fsmGameplay.SetPosition(new Vector2Int(
                        Mathf.FloorToInt(position.x + 0.5f),
                        Mathf.FloorToInt(position.y + 0.5f)
                    ));
                }

                if (CastleViewModel.IsPosition(position))
                    Debug.Log(" Это крепость " + CastleViewModel.ConfigId);
                
                //Клик за пределами башен
                //AreaViewModel.Hide();
                _entityClick.OnNext(Unit.Default);
                if (_fsmTower.IsSelected())
                {
                    _fsmTower.Fsm.SetState<FsmTowerNone>();
                }

                //Обработать другие состояния _fsmTower
                
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

            _entityClick.OnNext(Unit.Default);
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
        
    }
}