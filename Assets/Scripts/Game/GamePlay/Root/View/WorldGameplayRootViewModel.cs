using System;
using System.Collections;
using System.Linq;
using DI;
using Game.Common;
using Game.GamePlay.Classes;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.GameplayStates;
using Game.GamePlay.Fsm.HeroStates;
using Game.GamePlay.Fsm.SkillStates;
using Game.GamePlay.Fsm.TowerStates;
using Game.GamePlay.Services;
using Game.GamePlay.View.Castle;
using Game.GamePlay.View.Frames;
using Game.GamePlay.View.Frames.BuildFrames;
using Game.GamePlay.View.Frames.HeroFrames;
using Game.GamePlay.View.Frames.SkillFrames;
using Game.GamePlay.View.Frames.TowerFrames;
using Game.GamePlay.View.Grounds;
using Game.GamePlay.View.Hero;
using Game.GamePlay.View.Map;
using Game.GamePlay.View.Mobs;
using Game.GamePlay.View.Roads;
using Game.GamePlay.View.Skills;
using Game.GamePlay.View.Towers;
using Game.GamePlay.View.Warriors;
using Game.GamePlay.View.Waves;
using Game.State;
using Game.State.Gameplay;
using Game.State.Gameplay.Rewards;
using ObservableCollections;
using R3;
using Scripts.Utils;
using UnityEngine;

namespace Game.GamePlay.Root.View
{
    public class WorldGameplayRootViewModel : IDisposable
    {
        public readonly IObservableCollection<TowerViewModel> AllTowers;

        //public readonly IObservableCollection<WarriorViewModel> AllWarriors;
        public readonly IObservableCollection<MobViewModel> AllMobs;
        public readonly IObservableCollection<GroundViewModel> AllGrounds;
        public readonly IObservableCollection<BoardViewModel> AllBoards;
        public readonly IObservableCollection<RoadViewModel> AllRoads;
        public readonly IObservableCollection<SkillViewModel> AllSkills;
        
        
        public readonly ReadOnlyReactiveProperty<FrameBlockViewModel> FrameBlockViewModel;
        //TODO Переделать на ReactiveProperty
        public readonly IObservableCollection<FramePlacementViewModel> FramePlacementViewModels;
        public readonly IObservableCollection<FrameSkillViewModel> FrameSkillViewModels;

        public readonly ReadOnlyReactiveProperty<FrameHeroViewModel> FrameHeroViewModel;
        
        public CastleViewModel CastleViewModel { get; private set; }
        public HeroViewModel HeroViewModel { get; }
        public GateWaveViewModel GateWaveViewModel { get; private set; }

        public GateWaveViewModel GateWaveSecondViewModel { get; private set; }

        //public AttackAreaViewModel AreaViewModel { get; }
        public MapFogViewModel MapFogViewModel { get; }

        private readonly FsmGameplay _fsmGameplay;
        private readonly FsmTower _fsmTower;
        private readonly FrameService _frameService;
        private readonly FramePlacementService _framePlacementService;
        private readonly WaveService _waveService;
        private readonly GameplayCamera _cameraService;

        private readonly DamageService _damageService;

        //Публичны, для передачи из RootBinder в те Binder, где модели необходимо реагировать на события клик
        private readonly Subject<Unit> _entityClick;
        private readonly Subject<TowerViewModel> _towerClick;
        private bool _isFrameDownClick; //Отслеживаем что перетаскивать Фрейм ил Камеру

        private readonly GameplayStateProxy _gameplayState;
        private readonly FsmSkill _fsmSkill;
        private readonly FrameSkillService _frameSkillService;
        
        private readonly TowersService _towersService;
        private readonly HeroesService _heroesService;
        private readonly FsmHero _fsmHero;
        private readonly FrameHeroService _frameHeroService;
        
        private DisposableBag _disposables;
        //  private readonly Coroutines _coroutines;

        public WorldGameplayRootViewModel(DIContainer container)
        {
            var groundsService = container.Resolve<GroundsService>();
            var castleService =container.Resolve<CastleService>();
                        var placementService = container.Resolve<PlacementService>();
                        var roadsService = container.Resolve<RoadsService>();
                        var skillsService = container.Resolve<SkillsService>();
                        
            _fsmGameplay = container.Resolve<FsmGameplay>();
            _fsmTower = container.Resolve<FsmTower>();
            _fsmSkill = container.Resolve<FsmSkill>();
            _fsmHero = container.Resolve<FsmHero>();
            
            _towersService = container.Resolve<TowersService>();
            
            _frameService = container.Resolve<FrameService>();
            _framePlacementService = container.Resolve<FramePlacementService>();
            _frameSkillService = container.Resolve<FrameSkillService>();
            _frameHeroService = container.Resolve<FrameHeroService>();

            _waveService = container.Resolve<WaveService>();
            _cameraService = container.Resolve<GameplayCamera>();
            _damageService = container.Resolve<DamageService>();
            
            _heroesService = container.Resolve<HeroesService>();
            
            
            //клики по объектам игрового мира, не UI 
            _entityClick = container.Resolve<Subject<Unit>>(AppConstants.CLICK_WORLD_ENTITY);
            _towerClick = container.Resolve<Subject<TowerViewModel>>();

            _gameplayState = container.Resolve<IGameStateProvider>().GameplayState;

            AllRoads = roadsService.AllRoads;
            AllGrounds = groundsService.AllGrounds;
            AllBoards = groundsService.AllBoards;
            AllTowers = _towersService.AllTowers;
            AllMobs = _waveService.AllMobsOnWay;
            AllSkills = skillsService.AllSkills;
            
            //AllWarriors = warriorService.AllWarriors;

            FrameBlockViewModel = _frameService.CurrentFrame;
            FramePlacementViewModels = _framePlacementService.ViewModels;
            FrameSkillViewModels = _frameSkillService.ViewModels;
            CastleViewModel = castleService.CastleViewModel;
            GateWaveViewModel = _waveService.GateWaveViewModel;
            GateWaveSecondViewModel = _waveService.GateWaveSecondViewModel;
            HeroViewModel = _heroesService.HeroViewModel;
            FrameHeroViewModel = _frameHeroService.ViewModel;

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
                        var level = _towersService.GameplayLevels[reward.ConfigId];
                        _entityClick.OnNext(Unit.Default);
                        _frameService.CreateFrameTower(position, level, reward.ConfigId);
                    }

                    if (reward.RewardType == RewardType.Road)
                    {
                        position = placementService.GetNewPositionRoad();
                        _frameService.CreateFrameRoad(position, reward.ConfigId, placementService.GetNewDirectionRoad());
                    }

                    if (reward.RewardType == RewardType.Ground)
                    {
                        position = placementService.GetNewPositionGround();
                        _frameService.CreateFrameGround(position);
                    }

                    if (reward.RewardType == RewardType.TowerMove)
                    {
                        position = AllTowers.First().GetPosition(); //Проверка если нет башен
                        _fsmGameplay.SelectFirstTower.Value = null;
                        //Перемещаем башню, сделать
                    }

                    //position вычисляется для разных сущностей, сразу допустимое значение
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
                            _frameService.RemoveFrameAnimation().Where(x => x).Subscribe(_ =>
                            {
                                _towersService.PlaceTower(card.ConfigId, position);
                                _fsmGameplay.Fsm.SetState<FsmStateGamePlay>();
                            });
                            break;
                        case RewardType.Ground:
                            //Без пыли
                            _frameService.RemoveFrame();
                            foreach (var groundPosition in _frameService.GetGrounds())
                            {
                                groundsService.PlaceGround(groundPosition);
                            }

                            groundsService.PlaceGround(position);
                            _fsmGameplay.Fsm.SetState<FsmStateGamePlay>();
                            break;
                        case RewardType.Road:
                            _frameService.RemoveFrameAnimation().Where(x => x).Subscribe(_ =>
                            {
                                var isMainPath = _frameService.IsMainPath();
                                foreach (var road in _frameService.GetRoadsForBuild())
                                {
                                    roadsService.PlaceRoad(road.Position, road.IsTurn, road.Rotate, isMainPath);
                                    groundsService.PlaceGround(road.Position);
                                }

                                _fsmGameplay.Fsm.SetState<FsmStateGamePlay>();
                            });
                            break;
                        case RewardType.TowerLevelUp:
                            _towersService.LevelUpTower(card.ConfigId)
                                .Where(x => x)
                                .Subscribe(_ => _fsmGameplay.Fsm.SetState<FsmStateGamePlay>());
                            break;
                        case RewardType.TowerMove:
                            _frameService.RemoveFrameAnimation().Where(x => x).Subscribe(_ =>
                            {
                                if (_fsmGameplay.SelectFirstTower.CurrentValue == null) throw new Exception("Ошибка");
                                _towersService.MoveTower(_fsmGameplay.SelectFirstTower.CurrentValue.UniqueId, position);
                                _fsmGameplay.SelectFirstTower.Value = null;
                                _fsmGameplay.Fsm.SetState<FsmStateGamePlay>();
                            });
                            break;
                        case RewardType.TowerReplace:
                            if (_fsmGameplay.SelectFirstTower.CurrentValue == null ||
                                _fsmGameplay.SelectSecondTower.CurrentValue == null
                               ) throw new Exception("Ошибка");
                            _towersService.ReplaceTower(
                                _fsmGameplay.SelectFirstTower.CurrentValue.UniqueId,
                                _fsmGameplay.SelectSecondTower.CurrentValue.UniqueId);
                            _towersService.UnSelectToReplace();
                            _fsmGameplay.Fsm.SetState<FsmStateGamePlay>();
                            break;
                        case RewardType.SkillLevelUp:
                            skillsService.LevelUpSkill(card.ConfigId);
                            _fsmGameplay.Fsm.SetState<FsmStateGamePlay>();
                            break;
                        case RewardType.HeroLevelUp:
                            _heroesService.LevelUpHero();
                            _fsmGameplay.Fsm.SetState<FsmStateGamePlay>();
                            break;
                        default: throw new Exception($"Неверный тип награды {card.RewardType}");
                    }
                }

                //Режим начала строительства/выбора наград
                if (newState.GetType() == typeof(FsmStateBuildBegin))
                {
                    if (newState.Fsm.PreviousState.GetType() == typeof(FsmStateBuild)) //Возврат от режима строим
                    {
                        _frameService.RemoveFrame();
                        _towersService.UnSelectToReplace();
                        _fsmGameplay.SelectFirstTower.Value = null;
                        _fsmGameplay.SelectSecondTower.Value = null;
                    }
                    //Удаляем фреймы
                }
            }).AddTo(ref _disposables);

            _fsmGameplay.Position.Subscribe(newPosition =>
            {
                if (_fsmGameplay.IsStateBuild())
                {
                    _frameService.MoveFrame(newPosition); //Переносим объект
                }

                if (_fsmGameplay.IsStateGaming())
                {
                    // _cameraService.MoveCamera(Vector2Int.zero); //Центрируем карту
                }

                if (_fsmTower.IsPlacement())
                {
                    _framePlacementService.MoveFrame(newPosition);
                }

                if (_fsmHero.IsPlacement())
                {
                    _frameHeroService.MoveFrame(newPosition);
                }
            }).AddTo(ref _disposables);
            
        }

        public void ClickEntity(Vector2 mousePosition)
        {
            var position = _cameraService.GetWorldPoint(mousePosition);

            //TODO Сделать проверку на пересечение состояний

            //Если Игра или Начало строительства
            //В этих же режим проходят все состояния FsmTower 
            if (_fsmGameplay.IsStateGaming() || _fsmGameplay.IsStateBuildBegin())
            {
                //И с башней нет работы или выделена (для смены на другую) или не работа с героем и навыками
                if ((_fsmTower.IsNone() || _fsmTower.IsSelected()) && _fsmSkill.IsNone() && !_fsmHero.IsSelected())
                {
                    if (_towersService.FindTowerByPosition(position, out var towerViewModel))
                    {
                        if (_fsmTower.IsSelected()) _fsmTower.Fsm.SetState<FsmTowerNone>(); //Сбрасываем выделение.
                        _fsmTower.Fsm.SetState<FsmTowerSelected>(towerViewModel); //Башня выделена
                        _cameraService.MoveCamera(towerViewModel.Position.Value);
                        return;
                    }
                }

                //Перемещение фреймов по клику 
                if (_fsmTower.IsPlacement())
                {
                    _fsmGameplay.SetPosition(new Vector2Int(
                        Mathf.FloorToInt(position.x + 0.5f),
                        Mathf.FloorToInt(position.y + 0.5f)
                    ));
                }
                if (_fsmHero.IsPlacement())
                {
                    _fsmGameplay.SetPosition(new Vector2Int(
                        Mathf.FloorToInt(position.x + 0.5f),
                        Mathf.FloorToInt(position.y + 0.5f)
                    ));
                }

                if (CastleViewModel.IsPosition(position))
                    Debug.Log(" Это крепость " + CastleViewModel.ConfigId);

                if (HeroViewModel.IsPosition(position) && !_fsmHero.IsSelected())
                {
                    _fsmHero.Fsm.SetState<FsmHeroSelected>();
                    return;
                }
                
                //Клик за пределами герой
                if (_fsmHero.IsSelected() && !HeroViewModel.IsPosition(position))
                {
                    _fsmHero.Fsm.SetState<FsmHeroUnSelected>();
                }
                
                if (_fsmTower.IsSelected())
                {
                    _fsmTower.Fsm.SetState<FsmTowerNone>();
                }
                
                //Клик за пределами башен
                //AreaViewModel.Hide();
                _entityClick.OnNext(Unit.Default);
                //Обработать другие состояния _fsmTower
                return;
            }

            //В режиме строительства обработка только фреймов и перемещение камеры
            if (_fsmGameplay.IsStateBuild())
            {
                _fsmGameplay.SetPosition(new Vector2Int(
                    Mathf.FloorToInt(position.x + 0.5f),
                    Mathf.FloorToInt(position.y + 0.5f)
                ));
                var card = _fsmGameplay.GetReward();
                //Режим перемещения башни и башня не выбрана
                if (card.RewardType == RewardType.TowerMove && _fsmGameplay.SelectFirstTower.Value == null)
                {
                    if (_towersService.FindTowerByPosition(position, out var towerViewModel))
                    {
                        _fsmGameplay.SelectFirstTower.Value = towerViewModel;
                        _cameraService.MoveCamera(towerViewModel.Position.Value);

                        _frameService.CreateFrameFromExistingTower(towerViewModel);
                    }
                    return;
                }

                //Режим обмена башнями
                if (card.RewardType == RewardType.TowerReplace)
                {
                    if (_fsmGameplay.SelectFirstTower.Value == null) //Выделяем первую башню
                    {
                        if (_towersService.FindTowerByPosition(position, out var towerViewModel))
                        {
                            _fsmGameplay.SelectFirstTower.Value = towerViewModel;
                            //TODO Отметить башни, которые можно выделить
                            _towersService.SelectToReplace(towerViewModel.IsOnRoad);
                            towerViewModel.Selected();
                        }
                        return;
                    }
                    
                    if (_fsmGameplay.SelectFirstTower.Value != null) //Выделяем вторую башню
                    {
                        if (_towersService.FindTowerByPosition(position, out var towerViewModel))
                        {
                            if (_fsmGameplay.SelectFirstTower.Value == towerViewModel) return;
                            _fsmGameplay.SelectSecondTower.Value?.UnSelected();
                            _fsmGameplay.SelectSecondTower.Value = towerViewModel;

                            towerViewModel.Selected();
                        }
                        return;
                    }
                }
                
                
                card.Position.x = Mathf.FloorToInt(position.x + 0.5f);
                card.Position.y = Mathf.FloorToInt(position.y + 0.5f);
                _fsmGameplay.Fsm.SetParam(card);
                //_fsmGameplay.Fsm.StateCurrent.Value.Params = card;
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


            //Запущен режим Навыка
            if (_fsmSkill.IsBegin())
            {
                _fsmSkill.Fsm.SetState<FsmSkillSetTarget>();
                _fsmSkill.SetPosition(position);
                return;
            }
            
            if (_fsmTower.IsPlacement())
            {
                //Попытаться выделить, если совпали координаты
                if (_framePlacementService.TrySelectedFrame(vectorInt)) return;
            }

            if (_fsmHero.IsPlacement())
            {
                if (_frameHeroService.TrySelectedFrame(vectorInt)) return;
            }
            
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

            //Считаем время в игре
            if (Time.timeScale > 0) _gameplayState.TotalTimeInScene.Value += Time.deltaTime;
        }

        public void FinishMoving(Vector2 mousePosition)
        {
            
            //Попытаться снять выделение, если был выделен
            if (_fsmTower.IsPlacement() && _framePlacementService.TryUnSelectedFrame()) return;
            if (_fsmHero.IsPlacement() && _frameHeroService.TryUnSelectedFrame()) return;
            

            if (_fsmSkill.IsTarget())
            {
                //Дублируем позицию размещения
                var position = _cameraService.GetWorldPoint(mousePosition);
                _fsmSkill.SetPosition(position);
                
                //Если Навык можно применить/разместить
                if (_frameSkillService.IsPlacement.CurrentValue)
                {
                    _fsmSkill.Fsm.SetState<FsmSkillShowEffect>();
                }
                else //Отменяем применение навыки
                {
                    _fsmSkill.Fsm.SetState<FsmSkillNone>();
                }
                return;
            }

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
            if (_framePlacementService.IsSelected())
            {
                var position = _cameraService.GetWorldPoint(mousePosition);
                var vectorInt = new Vector2Int(
                    Mathf.FloorToInt(position.x + 0.5f),
                    Mathf.FloorToInt(position.y + 0.5f)
                );

                _framePlacementService.MoveFrame(vectorInt);
                return;
            }

            if (_frameHeroService.IsSelected())
            {
                var position = _cameraService.GetWorldPoint(mousePosition);
                var vectorInt = new Vector2Int(
                    Mathf.FloorToInt(position.x + 0.5f),
                    Mathf.FloorToInt(position.y + 0.5f)
                );

                _frameHeroService.MoveFrame(vectorInt);
                return;
            }
            if (_fsmSkill.IsTarget())
            {
                var position = _cameraService.GetWorldPoint(mousePosition);
                _fsmSkill.SetPosition(position);
                return;
            }
            
            if (_isFrameDownClick) //Двигаем фрейм или показываем инфо 
            {
                ClickEntity(mousePosition);
            }
            else //Двигаем камеру
            {
                _cameraService.OnPointMove(mousePosition);
            }
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}