using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Game.GamePlay.Classes;
using Game.GamePlay.View.Buildings;
using Game.GamePlay.View.Castle;
using Game.GamePlay.View.Frames;
using Game.GamePlay.View.Grounds;
using Game.GamePlay.View.Mobs;
using Game.GamePlay.View.Roads;
using Game.GamePlay.View.Towers;
using Game.GamePlay.View.Waves;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using Scripts.Utils;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


namespace Game.GamePlay.Root.View
{
    public class WorldGameplayRootBinder : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform cameraSystem;

        //      [SerializeField] private BuildingBinder _prefabBuilding;
        //    private readonly Dictionary<int, BuildingBinder> _createBuildingsMap = new();
        private readonly Dictionary<int, TowerBinder> _createTowersMap = new();
        private readonly Dictionary<int, GroundBinder> _createGroundsMap = new();
       // private readonly Dictionary<int, FrameBlockBinder> _createFrameMap = new();
        private FrameBlockBinder _frameBlockBinder;
        private readonly Dictionary<int, RoadBinder> _createdRoadsMap = new();
        private readonly Dictionary<int, MobBinder> _createMobsMap = new();
        private readonly List<GateWaveBinder> _createGateMap = new();
        private CastleBinder _castleBinder;
        private readonly CompositeDisposable _disposables = new();

//private Coroutines _coroutines;
        private bool _clickCoroutines = false;
        private bool _isMouseDown;

        private GameplayCamera _gameplayCamera;
        private WorldGameplayRootViewModel _viewModel;

        private bool _isFrameDownClick = false; //Для перемещения FrameBlock фиксируем нажатие
        
        public void Bind(WorldGameplayRootViewModel viewModel)
        {
           
            _viewModel = viewModel;
            //1. Создаем все объекты мира из Прехабов
            //2. Подписываемся на добавление объектов в список (Создать) и на удаление (Уничтожить)

            //Поверхность уровня
            foreach (var groundViewModel in viewModel.AllGrounds)
                CreateGround(groundViewModel);
            _disposables.Add(
                viewModel.AllGrounds.ObserveAdd().Subscribe(e => CreateGround(e.Value))
            );
            _disposables.Remove(
                viewModel.AllGrounds.ObserveRemove().Subscribe(e => DestroyGround(e.Value))
            );
            //Башни
            foreach (var towerViewModel in viewModel.AllTowers)
                CreateTower(towerViewModel);
            _disposables.Add(
                viewModel.AllTowers.ObserveAdd().Subscribe(e =>
                {
                    CreateTower(e.Value);
                    
                })
            );
            _disposables.Remove(
                viewModel.AllTowers.ObserveRemove().Subscribe(e => DestroyTower(e.Value))
            );
            //Мобы
            foreach (var mobViewModel in viewModel.AllMobs)
                CreateMob(mobViewModel);
            _disposables.Add(
                viewModel.AllMobs.ObserveAdd().Subscribe(e =>
                {
                    CreateMob(e.Value);
                })
            );
            _disposables.Remove(
                viewModel.AllMobs.ObserveRemove().Subscribe(e => DestroyMob(e.Value))
            );
            
            //Замок
            CreateCastle(viewModel.CastleViewModel);
            //Дорога
            foreach (var roadViewModel in viewModel.AllRoads)
                CreateRoad(roadViewModel);
            _disposables.Add(
                viewModel.AllRoads.ObserveAdd().Subscribe(e => CreateRoad(e.Value))
            );
            _disposables.Remove(
                viewModel.AllRoads.ObserveRemove().Subscribe(e => DestroyRoad(e.Value))
            );
            
            //Фрейм строительный //только подписка, в начале уровня его нет
            _disposables.Add(
                viewModel.FrameBlockViewModels.ObserveAdd().Subscribe(e => CreateFrameBlock(e.Value))
                );
            _disposables.Remove(
                viewModel.FrameBlockViewModels.ObserveRemove().Subscribe(e => DestroyFrameBlock(e.Value))
                );
            
            
            _gameplayCamera = new GameplayCamera(_camera, cameraSystem);
           _viewModel.CameraMove.Subscribe(newValue =>
            {
                _gameplayCamera.MoveCamera(newValue);
            });
           //Создаем view-модель ворот из прехаба
           CreateGateWave(_viewModel.GateWaveViewModel);
           CreateGateWave(_viewModel.GateWaveViewModelSecond);

           //Запускаем следующую волну
            _viewModel.StartNextWave();

        }

        private void OnDestroy()
        {
            if (_castleBinder != null)
            {
                Destroy(_castleBinder.gameObject);
            }
            _disposables.Dispose();
            _createGateMap.ForEach(item => Destroy(item.gameObject));
        }

        private void CreateGateWave(GateWaveViewModel viewModel)
        {
            if (viewModel == null) return;
            var prefabPath = $"Prefabs/Gameplay/GateWave"; //Перенести в настройки уровня
            var gatePrefab = Resources.Load<GateWaveBinder>(prefabPath);
            var createdGate = Instantiate(gatePrefab, transform);
            createdGate.Bind(viewModel);
            _createGateMap.Add(createdGate);
        }

        private void CreateMob(MobViewModel mobViewModel)
        {
            var prefabPath = $"Prefabs/Gameplay/Mobs/{mobViewModel.ConfigId}"; //Перенести в настройки уровня
            var mobPrefab = Resources.Load<MobBinder>(prefabPath);
            var createdMob = Instantiate(mobPrefab, transform);
            createdMob.Bind(mobViewModel);
            //_castleBinder = createdCastle;
            _createMobsMap[mobViewModel.MobEntityId] = createdMob;
        }
        
        private void CreateCastle(CastleViewModel castleViewModel)
        {
            var prefabPath = "Prefabs/Gameplay/Buildings/Castle"; //Перенести в настройки уровня
            var castlePrefab = Resources.Load<CastleBinder>(prefabPath);
            var createdCastle = Instantiate(castlePrefab, transform);
            createdCastle.Bind(castleViewModel);
            _castleBinder = createdCastle;
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
         //   Debug.Log("CreateRoad = " + roadViewModel.Position.CurrentValue.x + " " + roadViewModel.Position.CurrentValue.y);
            var roadConfig = roadViewModel.ConfigId;
            var direction = roadViewModel.IsTurn ? "Turn" : "Line";
            var prefabRoadLevelPath =
                $"Prefabs/Gameplay/Roads/{roadConfig}{direction}";
            var roadPrefab = Resources.Load<RoadBinder>(prefabRoadLevelPath);
            var createdRoad = Instantiate(roadPrefab, parentTransform ?? transform);
 
            createdRoad.Bind(roadViewModel);
            _createdRoadsMap[roadViewModel.RoadEntityId] = createdRoad;

        }
        private void DestroyRoad(RoadViewModel roadViewModel)
        {
            if (_createdRoadsMap.TryGetValue(roadViewModel.RoadEntityId, out var roadBinder))
            {
                Destroy(roadBinder.gameObject);
                _createdRoadsMap.Remove(roadViewModel.RoadEntityId);
            }
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
                foreach (var groundFrameViewModel in frameBlockViewModel.EntityViewModels.Cast<GroundFrameViewModel>().ToList())
                {
                    CreateGroundFrame(groundFrameViewModel, createdFrame.transform);
                }
            }
        }

        private void CreateGroundFrame(GroundFrameViewModel groundFrameViewModel, Transform parentTransform)
        {
            //var groundType = groundViewModel.ConfigId;
            //var odd = Math.Abs((groundViewModel.Position.CurrentValue.x + groundViewModel.Position.CurrentValue.y) % 2);
            var prefabGroundFramePath = $"Prefabs/Gameplay/Grounds/Frame"; //Перенести в настройки уровня
            var groundFramePrefab = Resources.Load<GroundFrameBinder>(prefabGroundFramePath);
            var createdGroundFrame = Instantiate(groundFramePrefab, parentTransform);
            createdGroundFrame.Bind(groundFrameViewModel);
           // _createGroundsMap[groundViewModel.GroundEntityId] = createdGround;
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
        private void CreateGround(GroundViewModel groundViewModel)
        {
            var groundType = groundViewModel.ConfigId;
            var odd = Math.Abs((groundViewModel.Position.CurrentValue.x + groundViewModel.Position.CurrentValue.y) % 2);
            var prefabGroundPath = $"Prefabs/Gameplay/Grounds/{groundType}"; //Перенести в настройки уровня
            var groundPrefab = Resources.Load<GroundBinder>(prefabGroundPath);
            var createdGround = Instantiate(groundPrefab, transform);
            createdGround.Bind(groundViewModel, odd);
            _createGroundsMap[groundViewModel.GroundEntityId] = createdGround;
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
                    _isFrameDownClick = _viewModel.DownFrame(_gameplayCamera.GetWorldPoint(mousePosition));
                    
                    if (!_isFrameDownClick) //Иначе камеру
                        _gameplayCamera.OnPointDown(mousePosition); //Вызываем функцию начала перетаскивания
                }

                if (Input.GetMouseButton(0) && !_clickCoroutines)
                {
                    if (_isFrameDownClick) //Двигаем фрейм 
                    {
                        var position = _gameplayCamera.GetWorldPoint(mousePosition);
                        _viewModel.ClickEntity(position);
                        
                        //_viewModel.MoveFrame(_gameplayCamera.GetWorldPoint(mousePosition));
                    } else //Двигаем камеру
                    {
                        _gameplayCamera.OnPointMove(mousePosition); //Debug.Log("Мышь зажата");
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    if (_clickCoroutines) //  Это был "Клик"
                    {
                        _clickCoroutines = false;
                        var position = _gameplayCamera.GetWorldPoint(mousePosition);
                        _viewModel.ClickEntity(position);
                    }
                    else
                    {
                        if (_isFrameDownClick)
                        {
                            _viewModel.UpFrame(); //Завершаем движение фрейма
                        } else {
                            _gameplayCamera.OnPointUp(mousePosition); //Завершаем движение камеры
                        }
                    }
                }
            }

            _gameplayCamera?.UpdateMoving(); //Движение камеры
            _gameplayCamera?.AutoMoving();
            
            
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