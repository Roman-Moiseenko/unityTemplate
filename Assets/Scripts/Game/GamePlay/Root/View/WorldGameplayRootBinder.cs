using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.GamePlay.View.AttackAreas;
using Game.GamePlay.View.Castle;
using Game.GamePlay.View.Frames;
using Game.GamePlay.View.Grounds;
using Game.GamePlay.View.Mobs;
using Game.GamePlay.View.Roads;
using Game.GamePlay.View.Shots;
using Game.GamePlay.View.Towers;
using Game.GamePlay.View.Waves;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.GamePlay.Root.View
{
    public class WorldGameplayRootBinder : MonoBehaviour
    {
        //      [SerializeField] private BuildingBinder _prefabBuilding;
        //    private readonly Dictionary<int, BuildingBinder> _createBuildingsMap = new();
        private readonly Dictionary<int, TowerBinder> _createTowersMap = new();
        private readonly Dictionary<int, GroundBinder> _createGroundsMap = new();
        private readonly Dictionary<int, BoardBinder> _createBoardsMap = new();
        private FrameBlockBinder _frameBlockBinder;
        private readonly Dictionary<int, RoadBinder> _createdRoadsMap = new();
        private readonly Dictionary<int, MobBinder> _createMobsMap = new();
        private readonly Dictionary<int, ShotBinder> _createShotsMap = new();
        private readonly List<GateWaveBinder> _createGateMap = new();
        private CastleBinder _castleBinder;
        private AttackAreaBinder _attackAreaBinder;
        //private readonly CompositeDisposable _disposables = new();

        private bool _clickCoroutines = false;
        private bool _isMouseDown;
        private IDisposable _disposable;


        private Dictionary<string, List<MobBinder>> _mobsPull = new(); //Пул мобов
        private Dictionary<string, List<ShotBinder>> _shotsPull = new(); //Пул выстрелов

        //private GameplayCamera _gameplayCamera;
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
                .Subscribe(e =>
                {
                    //Debug.Log("e.Value.BoardEntityId = " + e.Value.BoardEntityId);
                    CreateBoard(e.Value);
                    
                })
                .AddTo(ref d);
            viewModel.AllBoards.ObserveRemove()
                .Subscribe(e => DestroyBoard(e.Value))
                .AddTo(ref d);
            //Башни
            foreach (var towerViewModel in viewModel.AllTowers) CreateTower(towerViewModel);
            viewModel.AllTowers.ObserveAdd().Subscribe(e =>
            {
                // Debug.Log("Башня добавилась в список " + e.Value.ConfigId + e.Value.TowerEntityId);
                CreateTower(e.Value);
            }).AddTo(ref d);
            viewModel.AllTowers.ObserveRemove()
                .Subscribe(e => DestroyTower(e.Value))
                .AddTo(ref d);
            
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
                        var mobBinder = listMobs.Find(m => m._viewModel.MobEntityId == e.Value.MobEntityId);
                        mobBinder.FreeUp();
                    }
                })
                .AddTo(ref d);

            //Выстрелы
            foreach (var shotViewModel in viewModel.AllShots)
                CreateShot(shotViewModel);
            viewModel.AllShots.ObserveAdd()
                .Subscribe(e => FindFreeOrCreateShot(e.Value))
                .AddTo(ref d);
            viewModel.AllShots.ObserveRemove()
                .Subscribe(e => {
                    if (_shotsPull.TryGetValue(e.Value.ConfigId, out var listShots))
                    {
                        var shotBinder = listShots.Find(m => m._viewModel.ShotEntityId == e.Value.ShotEntityId);
                        shotBinder.FreeUp();
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

            //Фрейм строительный //только подписка, в начале уровня его нет
            viewModel.FrameBlockViewModels.ObserveAdd()
                .Subscribe(e => CreateFrameBlock(e.Value))
                .AddTo(ref d);
            viewModel.FrameBlockViewModels.ObserveRemove()
                .Subscribe(e => DestroyFrameBlock(e.Value))
                .AddTo(ref d);

            //Создаем view-модель ворот из прехаба
            CreateGateWave(_viewModel.GateWaveViewModel);
            CreateGateWave(_viewModel.GateWaveViewModelSecond);
            CreateAttackArea(_viewModel.AreaViewModel);

            //Запускаем следующую волну
            _viewModel.StartGameplayServices();
            _disposable = d.Build();
        }


        private void OnDestroy()
        {
            if (_castleBinder != null) Destroy(_castleBinder.gameObject);

            if (_attackAreaBinder != null) Destroy(_attackAreaBinder.gameObject);

          //  _disposables.Dispose();
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

        private void CreateShot(ShotViewModel shotViewModel)
        {
            if (shotViewModel.NotPrefab) return;
            var prefabPath = $"Prefabs/Gameplay/Shots/{shotViewModel.ConfigId}"; //Перенести в настройки уровня
            var shotPrefab = Resources.Load<ShotBinder>(prefabPath);
            var createdShot = Instantiate(shotPrefab, transform);
            createdShot.Bind(shotViewModel);
            _createShotsMap[shotViewModel.ShotEntityId] = createdShot;
            //Добавляем выстрел в пул
            if (_shotsPull.TryGetValue(shotViewModel.ConfigId, out var listBinders))
            {
                listBinders.Add(createdShot);
            }
            else
            {
                var listBinder = new List<ShotBinder> { createdShot };
                _shotsPull.Add(shotViewModel.ConfigId, listBinder);
            }
        }
        
        private void FindFreeOrCreateShot(ShotViewModel shotViewModel)
        {
            if (_shotsPull.TryGetValue(shotViewModel.ConfigId, out var listBinders))
            {
                foreach (var shotBinder in listBinders.Where(shotBinder => shotBinder.Free.Value))
                {
                    shotBinder.Bind(shotViewModel);
                    return;
                }
            }
            CreateShot(shotViewModel);
        }
        
        private void CreateMob(MobViewModel mobViewModel)
        {
            var prefabPath = $"Prefabs/Gameplay/Mobs/{mobViewModel.ConfigId}"; //Перенести в настройки уровня
            var mobPrefab = Resources.Load<MobBinder>(prefabPath);
            var createdMob = Instantiate(mobPrefab, transform);
            
            _createMobsMap[mobViewModel.MobEntityId] = createdMob;
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
            //return createdMob;
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

        private void CreateAttackArea(AttackAreaViewModel attackAreaViewModel)
        {
            var prefabPath = "Prefabs/Gameplay/AttackArea"; //Перенести в настройки уровня
            var areaPrefab = Resources.Load<AttackAreaBinder>(prefabPath);
            var createdArea = Instantiate(areaPrefab, transform);
            createdArea.Bind(attackAreaViewModel);
            _attackAreaBinder = createdArea;
        }

        private void CreateTower(TowerViewModel towerViewModel, Transform parentTransform = null)
        {
            var towerLevel = towerViewModel.Level;
            var towerType = towerViewModel.ConfigId;

            var prefabTowerLevelPath =
                $"Prefabs/Gameplay/Towers/{towerType}/Level_{towerLevel}"; //Перенести в настройки уровня
            var towerPrefab = Resources.Load<TowerBinder>(prefabTowerLevelPath);
            var createdTower = Instantiate(towerPrefab, parentTransform ?? transform);
            createdTower.Bind(towerViewModel);
            _createTowersMap[towerViewModel.TowerEntityId] = createdTower;
        }

        private void CreateRoad(RoadViewModel roadViewModel, Transform parentTransform = null)
        {
            var roadConfig = roadViewModel.ConfigId;
            var direction = roadViewModel.IsTurn ? "Turn" : "Line";
            var prefabRoadLevelPath =
                $"Prefabs/Gameplay/Roads/{roadConfig}{direction}";
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

            if (frameBlockViewModel.IsTower())
            {
                CreateTower((TowerViewModel)frameBlockViewModel.EntityViewModels[0], createdFrame.transform);
            }

            if (frameBlockViewModel.IsRoad())
            {
                foreach (var roadViewModel in frameBlockViewModel.EntityViewModels.Cast<RoadViewModel>().ToList())
                {
                    CreateRoad(roadViewModel, createdFrame.transform);
                }
            }

            if (frameBlockViewModel.IsGround())
            {
                foreach (var groundFrameViewModel in frameBlockViewModel.EntityViewModels.Cast<GroundFrameViewModel>()
                             .ToList())
                {
                    CreateGroundFrame(groundFrameViewModel, createdFrame.transform);
                }
            }
        }

        private void CreateGroundFrame(GroundFrameViewModel groundFrameViewModel, Transform parentTransform)
        {
            var prefabGroundFramePath = $"Prefabs/Gameplay/Grounds/Frame"; //Перенести в настройки уровня
            var groundFramePrefab = Resources.Load<GroundFrameBinder>(prefabGroundFramePath);
            var createdGroundFrame = Instantiate(groundFramePrefab, parentTransform);
            createdGroundFrame.Bind(groundFrameViewModel);
        }

        //DESTROY

        private void DestroyRoad(RoadViewModel roadViewModel)
        {
            if (_createdRoadsMap.TryGetValue(roadViewModel.RoadEntityId, out var roadBinder))
            {
                Destroy(roadBinder.gameObject);
                _createdRoadsMap.Remove(roadViewModel.RoadEntityId);
            }
        }

        private void DestroyFrameBlock(FrameBlockViewModel frameBlockViewModel)
        {
            if (frameBlockViewModel.IsTower())
            {
                DestroyTower((TowerViewModel)frameBlockViewModel.EntityViewModels[0]);
            }

            if (frameBlockViewModel.IsRoad())
            {
                foreach (var roadViewModel in frameBlockViewModel.EntityViewModels.Cast<RoadViewModel>().ToList())
                {
                    DestroyRoad(roadViewModel);
                }
            }

            Destroy(_frameBlockBinder.gameObject);
            Destroy(_frameBlockBinder);
        }

        private void DestroyShot(ShotViewModel shotViewModel)
        {
            if (_createShotsMap.TryGetValue(shotViewModel.ShotEntityId, out var shotBinder))
            {
                if (shotBinder != null)
                {
                    Destroy(shotBinder.gameObject);
                    _createShotsMap.Remove(shotViewModel.ShotEntityId);
                }
            }
        }

        private void DestroyTower(TowerViewModel towerViewModel)
        {
            if (_createTowersMap.TryGetValue(towerViewModel.TowerEntityId, out var towerBinder))
            {
                Destroy(towerBinder.gameObject);
                _createTowersMap.Remove(towerViewModel.TowerEntityId);
            }
        }

        private void DestroyMob(MobViewModel mobViewModel)
        {
            if (_createMobsMap.TryGetValue(mobViewModel.MobEntityId, out var mobBinder))
            {
                Destroy(mobBinder.gameObject);
                _createMobsMap.Remove(mobViewModel.MobEntityId);
                mobViewModel = null;
                // Destroy(mobViewModel);
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
            //TODO Добавить обработку Input.GetTouch(0)
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Vector2 mousePosition = Input.mousePosition;

                if (Input.GetMouseButtonDown(0) && !_clickCoroutines)
                {
                    StartCoroutine(IsClick());
                    return;
                }

                if (_isMouseDown) //Имитация GetMouseButtonDown
                {
                    _isMouseDown = false;
                    //Проверить куда нажали, если фрейм, перетаскиваем фрейм
                    _viewModel.StartMoving(mousePosition);
                }

                if (Input.GetMouseButton(0) && !_clickCoroutines)
                {
                    _viewModel.ProcessMoving(mousePosition);
                }

                if (Input.GetMouseButtonUp(0))
                {
                    if (_clickCoroutines) //  Это был "Клик"
                    {
                        _clickCoroutines = false;
                        _viewModel.ClickEntity(mousePosition);
                    }
                    else
                    {
                        _viewModel.FinishMoving(mousePosition);
                    }
                }
            }

            _viewModel?.Update();


            //  _gameplayCamera?.UpdateMoving(); //Движение камеры
            //  _gameplayCamera?.AutoMoving();


            // UpdateInput();

            /*    var position = Input.mousePosition;

                //Проверка мышки и состояния
                if (Input.GetMouseButtonDown(0))
                {
                    var cursorPosition = Input.mousePosition;
                 //   Ray ray = Camera.main.ScreenPointToRay(cursorPosition);
                 if (Physics.Raycast(Camera.main.ScreenPointToRay(cursorPosition), out RaycastHit hit))
                 {
                 //    print(hit.transform.name);
               //      print(hit.transform.position);
                 }

               //     Debug.Log(Input.mousePosition.x + ", "+ Input.mousePosition.y + ", " + Input.mousePosition.z);

                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    // _viewModel.HandleTestInput();
                }
                */
        }

        private void UpdateInput()
        {
#if UNITY_EDITOR
            if (!EventSystem.current.IsPointerOverGameObject())
            {
            }

#elif UNITY_IOS || UNITY_ANDROID
        Touch _touch = Input.GetTouch(0);
        if (!IsPointerOverUIObject()) {
            if (Input.touchCount > 0)
            {
                
                Vector2 touchPosition = _touch.position;
                if (_touch.phase == TouchPhase.Began) OnPointDown(touchPosition);
                if (_touch.phase == TouchPhase.Moved) OnPointMove(touchPosition);
                if (_touch.phase == TouchPhase.Ended) OnPointUp(touchPosition);
                if (_touch.phase == TouchPhase.Stationary) isDragging = false;
                
                var hit = new RaycastHit();
                for (var i = 0; i < Input.touchCount; ++i) {
                    if (Input.GetTouch(i).phase == TouchPhase.Began) {
                        var ray = camera.ScreenPointToRay(Input.GetTouch(i).position);
                        if (Physics.Raycast(ray, out hit)) {
                            hit.transform.gameObject.SendMessage("OnMouseDown");
                        }
                    }
                }
            }
        }
      /*  else
        {
            if (BlockPanel != TypeBlockPanelUI.None && _touch.phase == TouchPhase.Ended)
            {
                Messenger<TypeBlockPanelUI>.Broadcast(Events.TOUCH_SCREEN, BlockPanel);
                BlockPanel = TypeBlockPanelUI.None;
            }
        }*/
#endif
        }

        private bool IsPointerOverUIObject() //Проверка для Андроид - EventSystem.current.IsPointerOverGameObject()
        {
            if (Input.touchCount > 0)
            {
                Touch _touch = Input.GetTouch(0);
                var touchPosition = _touch.position;
                var eventData = new PointerEventData(EventSystem.current) { position = touchPosition };
                var results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventData, results);
                return results.Count > 0;
            }

            return false;
        }

        private IEnumerator IsClick()
        {
            _isMouseDown = false;
            _clickCoroutines = true;
            yield return new WaitForSeconds(0.1f);
            if (_clickCoroutines)
            {
                _isMouseDown = true;
                _clickCoroutines = false;
            }
        }
    }
}