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
using Game.GamePlay.View.Towers;
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
        private readonly Dictionary<int, FrameBinder> _createFrameMap = new();
        private CastleBinder _castleBinder;
        private readonly CompositeDisposable _disposables = new();

        private Coroutines _coroutines;
        private bool _clickCoroutines = false;
        private bool _isMouseDown;

        private GameplayCamera _gameplayCamera;
        private WorldGameplayRootViewModel _viewModel;


        public void Bind(WorldGameplayRootViewModel viewModel)
        {
            _coroutines = new GameObject("[COROUTINES]").AddComponent<Coroutines>();
            _viewModel = viewModel;
            //1. Создаем все объекты мира из Прехабов
            //2. Подписываемся на добавление объектов в список (Создать) и на удаление (Уничтожить)
            foreach (var towerViewModel in viewModel.AllTowers)
                CreateTower(towerViewModel);

        /*    viewModel.FramesBlock.Subscribe(newValue =>
            {
                Debug.Log(" --- ");
                if (newValue != null)
                {
                    Debug.Log("newValue.Position.CurrentValue.x" + newValue.Position.CurrentValue.x);
                    CreateFrameBlock(newValue);
                }
            });
            */
            _disposables.Add(
                viewModel.AllTowers.ObserveAdd().Subscribe(e =>
                {
                    CreateTower(e.Value); //Новая башня всегда создается как фрейм
                })
            );
            _disposables.Remove(
                viewModel.AllTowers.ObserveRemove().Subscribe(e => DestroyTower(e.Value))
            );
            _disposables.Add(viewModel.FramesBlock.ObserveAdd().Subscribe(e => CreateFrameBlock(e.Value)));
            _disposables.Remove(viewModel.FramesBlock.ObserveRemove().Subscribe(e => DestroyFrameBlock(e.Value)));
            //_disposables.Add(viewModel.AllFrames.ObserveAdd().Subscribe(e => {CreateFrame(e.Value); }));
            //_disposables.Remove(viewModel.AllFrames.ObserveRemove().Subscribe(e => {DestroyFrame(e.Value); }));
            //viewModel.AllTowers
            CreateCastle(viewModel.CastleViewModel);
/*
            foreach (var buildingViewModel in viewModel.AllBuildings)
                CreateBuilding(buildingViewModel);
            _disposables.Add(
                viewModel.AllBuildings.ObserveAdd().Subscribe(e => CreateBuilding(e.Value))
            );
            */


            foreach (var groundViewModel in viewModel.AllGrounds)
                CreateGround(groundViewModel);
            _disposables.Add(
                viewModel.AllGrounds.ObserveAdd().Subscribe(e => CreateGround(e.Value))
            );
            _disposables.Remove(
                viewModel.AllGrounds.ObserveRemove().Subscribe(e => DestroyGround(e.Value))
            );

            _gameplayCamera = new GameplayCamera(_camera, cameraSystem);
        }

        private void OnDestroy()
        {
            if (_castleBinder != null)
            {
                Destroy(_castleBinder.gameObject);
            }

            _disposables.Dispose();
        }

        private void DestroyFrames()
        {
            foreach (var frame in _createFrameMap)
            {
                //DestroyFrame(frame.)
            }
        }

        private void CreateCastle(CastleViewModel castleViewModel)
        {
            var prefabPath = "Prefabs/Gameplay/Buildings/Castle"; //Перенести в настройки уровня
            var castlePrefab = Resources.Load<CastleBinder>(prefabPath);
            var createdCastle = Instantiate(castlePrefab, transform);
            createdCastle.Bind(castleViewModel);
            _castleBinder = createdCastle;
        }

        private void CreateTower(TowerViewModel towerViewModel)
        {
            var towerLevel = towerViewModel.Level;
            var towerType = towerViewModel.ConfigId;

            var prefabTowerLevelPath =
                $"Prefabs/Gameplay/Towers/{towerType}/Level_{towerLevel}"; //Перенести в настройки уровня
            var towerPrefab = Resources.Load<TowerBinder>(prefabTowerLevelPath);

            var createdTower = Instantiate(towerPrefab, transform);
            createdTower.Bind(towerViewModel);
            _createTowersMap[towerViewModel.TowerEntityId] = createdTower;
        }

        private void CreateFrame(FrameViewModel frameViewModel)
        {
            var prefabFrame = "Prefabs/Gameplay/Frame";
            var framePrefab = Resources.Load<FrameBinder>(prefabFrame);
            var createdFrame = Instantiate(framePrefab, transform);
            createdFrame.Bind(frameViewModel);
            _createFrameMap.Add(frameViewModel.EntityId, createdFrame);
        }

        private void CreateFrameBlock(FrameBlock frameBlock)
        {
            if (frameBlock.FrameIs(FrameType.Tower))
            {
                CreateTower(frameBlock.As<FrameBlockTower>().TowerViewModel);
                CreateFrame(frameBlock.As<FrameBlockTower>().FrameViewModel);
            }
            
            //Создаем на карте строящийся объект
//            Debug.Log("Создаем объект FrameBlock");
        //    Debug.Log(JsonConvert.SerializeObject(frameBlock, Formatting.Indented));
        }

        private void DestroyFrameBlock(FrameBlock frameBlock)
        {
            if (frameBlock.FrameIs(FrameType.Tower))
            {
                DestroyTower(frameBlock.As<FrameBlockTower>().TowerViewModel);
                DestroyFrame(frameBlock.As<FrameBlockTower>().FrameViewModel);
            }
            //Создаем на карте строящийся объект
         //   Debug.Log("Удаляем объект FrameBlock");
        }

        private void CreateGround(GroundViewModel groundViewModel)
        {
            var groundType = groundViewModel.ConfigId;
            var odd = Math.Abs((groundViewModel.Position.CurrentValue.x + groundViewModel.Position.CurrentValue.y) % 2);
            var prefabGroundPath = $"Prefabs/Gameplay/Map/Grounds/{groundType}"; //Перенести в настройки уровня
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

        private void DestroyGround(GroundViewModel groundViewModel)
        {
            if (_createGroundsMap.TryGetValue(groundViewModel.GroundEntityId, out var groundBinder))
            {
                Destroy(groundBinder.gameObject);
                _createGroundsMap.Remove(groundViewModel.GroundEntityId);
            }
        }
        private void DestroyFrame(FrameViewModel frameViewModel)
        {
            if (_createFrameMap.TryGetValue(frameViewModel.EntityId, out var frameBinder))
            {
                Destroy(frameBinder.gameObject);
                _createFrameMap.Remove(frameViewModel.EntityId);
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
                    _coroutines.StartCoroutine(IsClick());
                    return;
                }

                if (_isMouseDown) //Имитация GetMouseButtonDown
                {
                    _isMouseDown = false;
                    _gameplayCamera.OnPointDown(mousePosition); //Вызываем функцию начала перетаскивания
                }

                if (Input.GetMouseButton(0) && !_clickCoroutines)
                {
                    _gameplayCamera.OnPointMove(mousePosition); //Debug.Log("Мышь зажата");
                }

                if (Input.GetMouseButtonUp(0))
                {
                    if (_clickCoroutines) //  Debug.Log("Клик");
                    {
                        _clickCoroutines = false;
                        var position = _gameplayCamera.GetWorldPoint(mousePosition);
                        _viewModel.ClickEntity(position);
                        /*
                          Ray ray = Camera.main.ScreenPointToRay(mousePosition);
                          RaycastHit rayHit;
                          if (Physics.Raycast(ray, out rayHit, 100.0f)) {

                                  var GameObjClicked = rayHit.collider.gameObject;
                                  Vector3 point = _camera.ScreenToWorldPoint(mousePosition);
                                  var position = _gameplayCamera.GetWorldPoint(mousePosition);
                                  Debug.Log( JsonUtility.ToJson(position));

                                  _viewModel.ClickEntity(position);
                          }
                          */
                    }
                    else
                    {
                        _gameplayCamera.OnPointUp(mousePosition); //Debug.Log("Мышь отпущена");
                    }
                }
            }

            _gameplayCamera?.UpdateMoving();

            // UpdateInput();

            //TODO Перемещение камеры
            ///
            ///

            //TODO В противном случае отправляем данные в контроллер
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
            yield return new WaitForSeconds(0.2f);
            if (_clickCoroutines)
            {
                _isMouseDown = true;
                _clickCoroutines = false;
            }
        }
    }
}