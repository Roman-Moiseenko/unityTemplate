using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Game.GamePlay.View.Grounds;
using Game.GamePlay.View.Roads;
using Game.GamePlay.View.Towers;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Frames
{
    public class FrameBlockBinder : MonoBehaviour
    {
        private FrameBlockViewModel _viewModel;
        
        [SerializeField] private Material allowed;
        [SerializeField] private Material allowedSelected;
        [SerializeField] private Material forbidden;
        [SerializeField] private Material forbiddenSelected;
        [SerializeField] private GameObject frame;
        [SerializeField] private Transform Element;
        [SerializeField] private ParticleSystem cloud;
        
        private Vector3 _targetPosition;
        private Vector3 _targetPositionElement;
        private bool _isMoving = false;
        private const int speed = 20;
        private const float smoothTime = 0.3f;
        private Vector3 _velocity;
        private IDisposable _disposable;

        private bool downElement = false;
        private bool showCloudDust = false;

        private List<MonoBehaviour> elements = new(); 
        
        public void Bind(FrameBlockViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            _viewModel = viewModel;
            var material = frame.GetComponent<Renderer>().material;
            
            Observable.Merge(viewModel.Enable, viewModel.IsSelected).Subscribe(v =>
            {
                material.SetInt("_Enabled", viewModel.Enable.CurrentValue ? 1 : 0);
                material.SetInt("_Selected", viewModel.IsSelected.CurrentValue ? 1 : 0);
                
            }).AddTo(ref d);
            viewModel.Position.Subscribe(newPosition =>
            {
                _targetPosition = new Vector3(newPosition.x, transform.position.y, newPosition.y);
                transform.DOMove(_targetPosition, smoothTime).SetEase(Ease.OutQuad).SetUpdate(true);
                
            }).AddTo(ref d);


            viewModel.Rotate
                .Subscribe(newValue => transform.localEulerAngles = new Vector3(0, 90f * newValue,0))
                .AddTo(ref d);
            
            transform.position = new Vector3(
                viewModel.Position.CurrentValue.x,
                transform.position.y,
                viewModel.Position.CurrentValue.y
            );

            viewModel.StartRemoveFlag.Where(x => x).Subscribe(_ =>
            {
                frame.SetActive(false);
                downElement = true;
                _targetPositionElement = Vector3.zero;
            }).AddTo(ref d);

            switch (viewModel.TypeElements)
            {
                case FrameType.Tower:
                    CreateTower((TowerViewModel)viewModel.EntityViewModels[0]);
                    break;
                case FrameType.Road:
                    foreach (var roadViewModel in viewModel.EntityViewModels.Cast<RoadViewModel>().ToList())
                        CreateRoad(roadViewModel);
                    
                    break;
                case FrameType.Ground:
                    foreach (var groundFrameViewModel in viewModel.EntityViewModels.Cast<GroundFrameViewModel>().ToList())
                        CreateGroundFrame(groundFrameViewModel);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            _disposable = d.Build();
        }
        
        private void Update()
        {
            if (_isMoving)
            {
                transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _velocity, smoothTime, speed, Time.unscaledTime);
                if (_velocity.magnitude < 0.0005)
                {
                    _isMoving = false;
                    transform.position = _targetPosition;
                }
            }

            if (downElement) //Опускаем Элементы
            {
                Element.localPosition = Vector3.Lerp(Element.localPosition, _targetPositionElement, 0.15f);
                if (Element.localPosition.y < 0.01)
                {
                    downElement = false;
                    showCloudDust = true; //Запуск пыли
                    cloud.Play();
                }
            }
            if (showCloudDust && !cloud.isPlaying)
            {
                showCloudDust = false;
                _viewModel.FinishRemoveFlag.OnNext(true);
            }
        }
        
        private void CreateTower(TowerViewModel towerViewModel)
        {
            var towerNumber = towerViewModel.NumberModel;
            var towerType = towerViewModel.ConfigId;
            var prefabTowerLevelPath = $"Prefabs/Gameplay/Towers/{towerType}/{towerType}-{towerNumber}"; //Перенести в настройки уровня
            var towerPrefab = Resources.Load<TowerBinder>(prefabTowerLevelPath);
            var createdTower = Instantiate(towerPrefab, Element.transform);
            createdTower.Bind(towerViewModel);
            elements.Add(createdTower);
        }
        private void CreateGroundFrame(GroundFrameViewModel groundFrameViewModel)
        {
            var prefabGroundFramePath = $"Prefabs/Gameplay/Grounds/Frame"; //Перенести в настройки уровня
            var groundFramePrefab = Resources.Load<GroundFrameBinder>(prefabGroundFramePath);
            var createdGroundFrame = Instantiate(groundFramePrefab, Element.transform);
            createdGroundFrame.Bind(groundFrameViewModel);
            elements.Add(createdGroundFrame);
        }
        private void CreateRoad(RoadViewModel roadViewModel)
        {
            var roadConfig = roadViewModel.ConfigId;
            var direction = roadViewModel.IsTurn ? "Turn" : "Line";
            var prefabRoadLevelPath = $"Prefabs/Gameplay/Roads/{roadConfig}{direction}";
            var roadPrefab = Resources.Load<RoadBinder>(prefabRoadLevelPath);
            var createdRoad = Instantiate(roadPrefab, Element.transform);
            createdRoad.Bind(roadViewModel);
            elements.Add(createdRoad);
        }
        
        private void OnDestroy()
        {
            _disposable.Dispose();
            foreach (var element in elements)
            {
                Destroy(element.gameObject);
                Destroy(element);
            }
        }
    }
}