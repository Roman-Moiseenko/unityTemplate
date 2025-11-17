using System;
using System.Collections.Generic;
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
        
        private readonly Dictionary<int, MapCardBinder> _createdMapCardMap = new();
        private Subject<int> _startLevelGame;
        private MapCardContainerViewModel _viewModel;
        IDisposable disposable;
        public void Bind(MapCardContainerViewModel viewModel, Subject<int> startLevelGame)
        {
            var d = Disposable.CreateBuilder();
            _startLevelGame = startLevelGame;
            _viewModel = viewModel;
            
            foreach (var mapViewModel in viewModel.AllMapsViewModels)
            {
                CreateMapCard(mapViewModel);
            }

            viewModel.LastMapId.Subscribe(v =>
            {
                if (v != 0) _createdMapCardMap[v].SetFinished();
                _createdMapCardMap[v + 1].SetEnabled();
            }).AddTo(ref d);
            disposable = d.Build();

        }
        
        
        private void CreateMapCard(MapCardViewModel viewModel)
        {
            var prefabMapCardPath =
                $"Prefabs/UI/MainMenu/ScreenPlay/MapCard";
            var mapPrefab = Resources.Load<MapCardBinder>(prefabMapCardPath);
            var createdMap = Instantiate(mapPrefab, content.GetComponent<Transform>());
            createdMap.Bind(viewModel, _startLevelGame);
            _createdMapCardMap[viewModel.MapId] = createdMap;
        }
        
        private void Start()
        {
            //screenView.OnScroll();
        }

        private void Update()
        {

        }
        public void OnScroll2()
        {
            Debug.Log("*****");
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