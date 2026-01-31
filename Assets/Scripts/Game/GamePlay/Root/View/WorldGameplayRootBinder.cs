using System;
using System.Collections.Generic;
using System.Linq;
using Game.GamePlay.Classes;
using Game.GamePlay.View.Castle;
using Game.GamePlay.View.Frames;
using Game.GamePlay.View.Grounds;
using Game.GamePlay.View.Map;
using Game.GamePlay.View.Mobs;
using Game.GamePlay.View.Roads;
using Game.GamePlay.View.Towers;
using Game.GamePlay.View.Warriors;
using Game.GamePlay.View.Waves;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.Root.View
{
    public class WorldGameplayRootBinder : MonoBehaviour
    {
        [SerializeField] private MapFogBinder mapFog;

        private readonly Dictionary<int, TowerBaseBinder> _createTowersMap = new();
        private readonly Dictionary<int, WarriorBinder> _createWarriorsMap = new();
        private readonly Dictionary<int, GroundBinder> _createGroundsMap = new();
        private readonly Dictionary<int, BoardBinder> _createBoardsMap = new();

        private FrameBlockBinder _frameBlockBinder;
        private FramePlacementBinder _framePlacementBinder;
        private readonly Dictionary<int, RoadBinder> _createdRoadsMap = new();
        private readonly Dictionary<int, MobBinder> _createMobsMap = new();
        private readonly List<GateWaveBinder> _createGateMap = new();
        private CastleBinder _castleBinder;
        // private AttackAreaBinder _attackAreaBinder;

        private IDisposable _disposable;
        private readonly Dictionary<string, List<MobBinder>> _mobsPull = new(); //Пул мобов
        private WorldGameplayRootViewModel _viewModel;

        public void Bind(WorldGameplayRootViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            _viewModel = viewModel;
            //1. Создаем все объекты мира из Прехабов
            //2. Подписываемся на добавление объектов в список (Создать) и на удаление (Уничтожить)

            //Поверхность уровня
            foreach (var groundViewModel in viewModel.AllGrounds) CreateGround(groundViewModel);
            viewModel.AllGrounds.ObserveAdd()
                .Subscribe(e => CreateGround(e.Value))
                .AddTo(ref d);
            viewModel.AllGrounds.ObserveRemove()
                .Subscribe(e => DestroyGround(e.Value))
                .AddTo(ref d);

            //Границы
            foreach (var boardViewModel in viewModel.AllBoards) CreateBoard(boardViewModel);
            viewModel.AllBoards.ObserveAdd()
                .Subscribe(e => CreateBoard(e.Value))
                .AddTo(ref d);
            viewModel.AllBoards.ObserveRemove()
                .Subscribe(e => DestroyBoard(e.Value))
                .AddTo(ref d);

            //Башни
            foreach (var towerViewModel in viewModel.AllTowers)
            {
                CreateTowerBase(towerViewModel);
            }

            viewModel.AllTowers.ObserveAdd().Subscribe(e => { CreateTowerBase(e.Value); }).AddTo(ref d);
            viewModel.AllTowers.ObserveRemove()
                .Subscribe(e => DestroyTowerBase(e.Value))
                .AddTo(ref d);

            //Воины
            foreach (var warriorViewModel in viewModel.AllWarriors)
            {
                CreateWarrior(warriorViewModel);
            }

            viewModel.AllWarriors.ObserveAdd().Subscribe(e => { CreateWarrior(e.Value); }).AddTo(ref d);
            viewModel.AllWarriors.ObserveRemove()
                .Subscribe(e => DestroyWarrior(e.Value))
                .AddTo(ref d);

            //

            //Мобы
            foreach (var mobViewModel in viewModel.AllMobs) CreateMob(mobViewModel);
            viewModel.AllMobs
                .ObserveAdd()
                .Subscribe(e => FindFreeOrCreateMob(e.Value))
                .AddTo(ref d);
            viewModel.AllMobs
                .ObserveRemove()
                .Subscribe(e =>
                {
                    if (_mobsPull.TryGetValue(e.Value.ConfigId, out var listMobs))
                    {
                        var mobBinder = listMobs.Find(m => m.ViewModel.UniqueId == e.Value.UniqueId);
                        mobBinder.FreeUp();
                    }
                })
                .AddTo(ref d);

            //Замок
            CreateCastle(viewModel.CastleViewModel);

            //Дорога
            foreach (var roadViewModel in viewModel.AllRoads) CreateRoad(roadViewModel);
            viewModel.AllRoads.ObserveAdd()
                .Subscribe(e => CreateRoad(e.Value))
                .AddTo(ref d);
            viewModel.AllRoads.ObserveRemove()
                .Subscribe(e => DestroyRoad(e.Value))
                .AddTo(ref d);

            //Фреймы

            //1. Фрейм строительный //только подписка, в начале уровня его нет
            viewModel.FrameBlockViewModels.ObserveAdd()
                .Subscribe(e => CreateFrameBlock(e.Value))
                .AddTo(ref d);
            viewModel.FrameBlockViewModels.ObserveRemove()
                .Subscribe(e => DestroyFrameBlock())
                .AddTo(ref d);

            //2. Фрейм расположения войск из башни
            viewModel.FramePlacement.Skip(1).Subscribe(framePlacement =>
            {
                if (framePlacement == null)
                {
                    DestroyFramePlacement();
                }
                else
                {
                    CreateFramePlacement(framePlacement);
                }
            });

            //Создаем view-модель ворот из прехаба
            CreateGateWave(_viewModel.GateWaveViewModel);
            CreateGateWave(_viewModel.GateWaveViewModelSecond);
            //CreateAttackArea(_viewModel.AreaViewModel);

            //Создаем Туман Войны
            mapFog.Bind(_viewModel.MapFogViewModel);

            //Запускаем следующую волну
            _viewModel.StartGameplayServices();
            _disposable = d.Build();
        }

        private void OnDestroy()
        {
            if (_castleBinder != null) Destroy(_castleBinder.gameObject);
            //   if (_attackAreaBinder != null) Destroy(_attackAreaBinder.gameObject);

            _disposable?.Dispose();
            _createGateMap.ForEach(item => Destroy(item.gameObject));
        }

        //CREATE 
        private void CreateGateWave(GateWaveViewModel viewModel)
        {
            if (viewModel == null) return;
            var prefabPath = $"Prefabs/Gameplay/Gate/GateWave"; //Перенести в настройки уровня
            var gatePrefab = Resources.Load<GateWaveBinder>(prefabPath);
            var createdGate = Instantiate(gatePrefab, transform);
            createdGate.Bind(viewModel);
            _createGateMap.Add(createdGate);
        }

        private void CreateWarrior(WarriorViewModel warriorViewModel)
        {
            var prefabWarriorPath =
                $"Prefabs/Gameplay/Warriors/Warrior-{warriorViewModel.ConfigId}"; //Перенести в настройки уровня
            var warriorPrefab = Resources.Load<WarriorBinder>(prefabWarriorPath);
            var createdWarrior = Instantiate(warriorPrefab, transform);
            createdWarrior.Bind(warriorViewModel);
            _createWarriorsMap[warriorViewModel.UniqueId] = createdWarrior;
        }

        private void CreateMob(MobViewModel mobViewModel)
        {
            var prefabPath = $"Prefabs/Gameplay/Mobs/{mobViewModel.ConfigId}"; //Перенести в настройки уровня
            var mobPrefab = Resources.Load<MobBinder>(prefabPath);
            var createdMob = Instantiate(mobPrefab, transform);

            _createMobsMap[mobViewModel.UniqueId] = createdMob;
            //Добавляем моба в пул
            if (_mobsPull.TryGetValue(mobViewModel.ConfigId, out var listBinders))
            {
                listBinders.Add(createdMob);
            }
            else
            {
                var listBinder = new List<MobBinder> { createdMob };
                _mobsPull.Add(mobViewModel.ConfigId, listBinder);
            }

            createdMob.Bind(mobViewModel);
        }

        private void FindFreeOrCreateMob(MobViewModel mobViewModel)
        {
            if (_mobsPull.TryGetValue(mobViewModel.ConfigId, out var listBinders))
            {
                foreach (var mobBinder in listBinders.Where(mobBinder => mobBinder.Free.Value))
                {
                    mobBinder.Bind(mobViewModel);
                    return;
                }
            }

            CreateMob(mobViewModel);
        }

        private void CreateCastle(CastleViewModel castleViewModel)
        {
            var prefabPath = "Prefabs/Gameplay/Buildings/Castle"; //Перенести в настройки уровня
            var castlePrefab = Resources.Load<CastleBinder>(prefabPath);
            var createdCastle = Instantiate(castlePrefab, transform);
            createdCastle.Bind(castleViewModel);
            _castleBinder = createdCastle;
        }

/*
        private void CreateAttackArea(AttackAreaViewModel attackAreaViewModel)
        {
            var prefabPath = "Prefabs/Gameplay/AttackArea/AttackArea"; //Перенести в настройки уровня
            var areaPrefab = Resources.Load<AttackAreaBinder>(prefabPath);
            var createdArea = Instantiate(areaPrefab, transform);
            createdArea.Bind(attackAreaViewModel);
            _attackAreaBinder = createdArea;
        }
*/
        private void CreateTowerBase(TowerViewModel towerViewModel)
        {
            var prefabTowerLevelPath = $"Prefabs/Gameplay/Towers/TowerBase"; //Перенести в настройки уровня
            var towerPrefab = Resources.Load<TowerBaseBinder>(prefabTowerLevelPath);
            var createdTower = Instantiate(towerPrefab, transform);
            createdTower.Bind(towerViewModel);
            _createTowersMap[towerViewModel.UniqueId] = createdTower;
        }

        private void CreateRoad(RoadViewModel roadViewModel, Transform parentTransform = null)
        {
            var roadConfig = roadViewModel.ConfigId;
            var direction = roadViewModel.IsTurn ? "Turn" : "Line";
            var prefabRoadLevelPath = $"Prefabs/Gameplay/Roads/{roadConfig}{direction}";
            var roadPrefab = Resources.Load<RoadBinder>(prefabRoadLevelPath);
            var createdRoad = Instantiate(roadPrefab, parentTransform ?? transform);

            createdRoad.Bind(roadViewModel);
            _createdRoadsMap[roadViewModel.RoadEntityId] = createdRoad;
        }

        private void CreateGround(GroundViewModel groundViewModel)
        {
            var prefabGroundPath = $"Prefabs/Gameplay/Grounds/Ground"; //Перенести в настройки уровня {groundType}
            var groundPrefab = Resources.Load<GroundBinder>(prefabGroundPath);
            var createdGround = Instantiate(groundPrefab, transform);
            createdGround.Bind(groundViewModel);
            _createGroundsMap[groundViewModel.GroundEntityId] = createdGround;
        }

        private void CreateBoard(BoardViewModel boardViewModel)
        {
            var prefabBoardPath = $"Prefabs/Gameplay/Grounds/Board"; //Перенести в настройки уровня {groundType}
            var boardPrefab = Resources.Load<BoardBinder>(prefabBoardPath);
            var createdBoard = Instantiate(boardPrefab, transform);
            createdBoard.Bind(boardViewModel);
            _createBoardsMap[boardViewModel.BoardEntityId] = createdBoard;
        }

        private void CreateFrameBlock(FrameBlockViewModel frameBlockViewModel)
        {
            var prefabFrame = $"Prefabs/Gameplay/Frames/block_{frameBlockViewModel.GetCountFrames()}";
            var framePrefab = Resources.Load<FrameBlockBinder>(prefabFrame);
            var createdFrame = Instantiate(framePrefab, transform);
            createdFrame.Bind(frameBlockViewModel);
            _frameBlockBinder = createdFrame;
        }

        private void CreateFramePlacement(FramePlacementViewModel framePlacementViewModel)
        {
            var prefabFrame = $"Prefabs/Gameplay/Frames/FramePlacement";
            var framePrefab = Resources.Load<FramePlacementBinder>(prefabFrame);
            var createdFrame = Instantiate(framePrefab, transform);
            createdFrame.Bind(framePlacementViewModel);
            _framePlacementBinder = createdFrame;
        }

        //DESTROY
        private void DestroyWarrior(WarriorViewModel warriorViewModel)
        {
            if (_createWarriorsMap.TryGetValue(warriorViewModel.UniqueId, out var warriorBinder))
            {
                Destroy(warriorBinder.gameObject);
                _createWarriorsMap.Remove(warriorViewModel.UniqueId);
            }
        }

        private void DestroyRoad(RoadViewModel roadViewModel)
        {
            if (_createdRoadsMap.TryGetValue(roadViewModel.RoadEntityId, out var roadBinder))
            {
                Destroy(roadBinder.gameObject);
                _createdRoadsMap.Remove(roadViewModel.RoadEntityId);
            }
        }

        private void DestroyFrameBlock()
        {
            Destroy(_frameBlockBinder.gameObject);
            Destroy(_frameBlockBinder);
        }

        private void DestroyFramePlacement()
        {
            Destroy(_framePlacementBinder.gameObject);
            Destroy(_framePlacementBinder);
        }

        private void DestroyTowerBase(TowerViewModel towerViewModel)
        {
            if (_createTowersMap.TryGetValue(towerViewModel.UniqueId, out var towerBinder))
            {
                Destroy(towerBinder.gameObject);
                _createTowersMap.Remove(towerViewModel.UniqueId);
            }
        }

        private void DestroyMob(MobViewModel mobViewModel)
        {
            if (_createMobsMap.TryGetValue(mobViewModel.UniqueId, out var mobBinder))
            {
                Destroy(mobBinder.gameObject);
                _createMobsMap.Remove(mobViewModel.UniqueId);
            }
        }

        private void DestroyGround(GroundViewModel groundViewModel)
        {
            if (_createGroundsMap.TryGetValue(groundViewModel.GroundEntityId, out var groundBinder))
            {
                Destroy(groundBinder.gameObject);
                _createGroundsMap.Remove(groundViewModel.GroundEntityId);
            }
        }

        private void DestroyBoard(BoardViewModel boardViewModel)
        {
            if (_createBoardsMap.TryGetValue(boardViewModel.BoardEntityId, out var boardBinder))
            {
                Destroy(boardBinder.gameObject);
                _createBoardsMap.Remove(boardViewModel.BoardEntityId);
            }
        }

        private void Update()
        {
            _viewModel?.Update();
        }

        /**
         * Функции для отловли событий на Input
         */
        private void HandleScaling(bool scalingUp)
        {
            _viewModel.ScalingCamera(scalingUp);
        }

        private void HandleTap(Vector2 screenPosition)
        {
            _viewModel.ClickEntity(screenPosition);
        }

        private void HandlePointerDown(Vector2 screenPosition)
        {
            _viewModel.StartMoving(screenPosition);
        }

        private void HandlePointerUp(Vector2 screenPosition)
        {
            _viewModel.FinishMoving(screenPosition);
        }

        private void HandlePointerDrag(Vector2 startPosition, Vector2 currentPosition)
        {
            _viewModel.ProcessMoving(currentPosition);
        }

        private void OnEnable()
        {
            InputManager.OnTapPerformed += HandleTap;
            InputManager.OnPointerDown += HandlePointerDown;
            InputManager.OnPointerUp += HandlePointerUp;
            InputManager.OnPointerDrag += HandlePointerDrag;
            InputManager.OnScalingUp += HandleScaling;
        }

        private void OnDisable()
        {
            InputManager.OnTapPerformed -= HandleTap;
            InputManager.OnPointerDown -= HandlePointerDown;
            InputManager.OnPointerUp -= HandlePointerUp;
            InputManager.OnPointerDrag -= HandlePointerDrag;
            InputManager.OnScalingUp -= HandleScaling;
        }
    }
}