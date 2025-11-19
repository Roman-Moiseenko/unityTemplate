using System;
using System.Collections.Generic;
using MVVM.Storage;
using Newtonsoft.Json;
using R3;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.MainMenu.View.ScreenPlay.MapsGo
{
    public class MapCardContainerBinder : MonoBehaviour
    {
        [SerializeField] private ScrollRect screenView;
        [SerializeField] private RectTransform viewPort;
        [SerializeField] public RectTransform content;
        [SerializeField] private HorizontalLayoutGroup hlg;

        [SerializeField] private Button btnLeft;
        [SerializeField] private Button btnRight;
        
        private readonly Dictionary<int, MapCardBinder> _createdMapCardMap = new();
        private Subject<int> _startLevelGame;
        private MapCardContainerViewModel _viewModel;
        private float _deltaScroll;
        IDisposable disposable;
        
        private bool _isScrolling = false;
        private float _targetScroll;
        private ReactiveProperty<int> _numberShow = new(1);
        private float startScroll = 0f;
        private StorageManager _storageManager;

        public void Bind(MapCardContainerViewModel viewModel, 
            Subject<int> startLevelGame, 
            StorageManager storageManager)
        {
            var d = Disposable.CreateBuilder();
            _startLevelGame = startLevelGame;
            _viewModel = viewModel;
            _storageManager = storageManager;
            
            foreach (var mapViewModel in viewModel.AllMapsViewModels)
            {
                CreateMapCard(mapViewModel);
            }

            viewModel.LastMapId.Subscribe(v =>
            {
                if (v != 0) _createdMapCardMap[v].SetFinished();
                _createdMapCardMap[v + 1].SetEnabled();
            }).AddTo(ref d);

            _deltaScroll = 1f / (_createdMapCardMap.Count - 1);
            _numberShow.Skip(1).Subscribe(v =>
            {
                _isScrolling = true;
                _targetScroll = _deltaScroll * (v - 1);
            }).AddTo(ref d);
            
            disposable = d.Build();
        }
        
        
        private void CreateMapCard(MapCardViewModel viewModel)
        {
            var prefabMapCardPath =
                $"Prefabs/UI/MainMenu/ScreenPlay/MapCard";
            var mapPrefab = Resources.Load<MapCardBinder>(prefabMapCardPath);
            var createdMap = Instantiate(mapPrefab, content.GetComponent<Transform>());
            createdMap.Bind(viewModel, _startLevelGame, _storageManager);
            _createdMapCardMap[viewModel.MapId] = createdMap;
        }

        private void OnEnable()
        {
            btnLeft.onClick.AddListener(OnScrollLeft);
            btnRight.onClick.AddListener(OnScrollRight);
        }

        private void Update()
        {
            if (_isScrolling)
            {
                //Движение 
                screenView.horizontalNormalizedPosition = Mathf.Lerp(
                    screenView.horizontalNormalizedPosition, _targetScroll, 1 / 10f);
                if (Mathf.Abs(screenView.horizontalNormalizedPosition - _targetScroll) < 0.0001)
                {
                    //Debug.Log("STOP SCROLL");
                    _isScrolling = false;
                }
                

            }
        }
        
        private void OnScrollRight()
        {
            if (_numberShow.CurrentValue < _createdMapCardMap.Count) _numberShow.Value++;
        }

        private void OnScrollLeft()
        {
            if (_numberShow.CurrentValue > 1) _numberShow.Value--;
        }

        private void OnDisable()
        {
            btnLeft.onClick.RemoveListener(OnScrollLeft);
            btnRight.onClick.RemoveListener(OnScrollRight);
        }

        private void Start()
        {
            //screenView.OnScroll();
        }

        public void OnScrollBegin(BaseEventData eventData)
        {
            startScroll = content.localPosition.x;
        }
        public void OnScrollEnd(BaseEventData eventData)
        {
            var endScroll = content.localPosition.x;
//            Debug.Log(startScroll + " > " + endScroll);
            if (startScroll - endScroll > 0)
            {
                OnScrollRight();
            }
            else
            {
                OnScrollLeft();
            }
        }
        public void OnScroll(BaseEventData eventData)
        {
            //eventData.selectedObject.GetComponent<ScrollRect>()
            Debug.Log(content.localPosition.x);
        }
        
        private void OnDestroy()
        {
            disposable.Dispose();
        }
    }
}