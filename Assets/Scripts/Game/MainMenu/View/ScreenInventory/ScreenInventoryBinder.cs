using System;
using System.Collections.Generic;
using Game.MainMenu.View.ScreenInventory.TowerCards;
using MVVM.UI;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenu.View.ScreenInventory
{
    public class ScreenInventoryBinder : WindowBinder<ScreenInventoryViewModel>
    {
        //    [SerializeField] private Button _btnGoToPlay;
        private IDisposable _disposable;
        [SerializeField] private Transform containerTowerCard;

        [SerializeField] private Button buttonTest;
        
        private readonly Dictionary<int, TowerCardBinder> _createdTowerCardMap = new();

        protected override void OnBind(ScreenInventoryViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            base.OnBind(viewModel);
            foreach (var towerCardViewModel in viewModel.TowerCards)
            {
                CreateTowerCard(towerCardViewModel);
            }

            viewModel.TowerCards.ObserveAdd().Subscribe(e =>
            {
                CreateTowerCard(e.Value);
            }).AddTo(ref d);
            viewModel.TowerCards.ObserveRemove().Subscribe(e =>
            {
                DestroyTowerCard(e.Value);
                //TODO Перестройка расположения меню
            }).AddTo(ref d);
            _disposable = d.Build();
        }

        private void CreateTowerCard(TowerCardViewModel viewModel)
        {
            var count = _createdTowerCardMap.Count;
           // Debug.Log(count);
            var col = count / 5;
            var row = count % 5;
            
            
            //TODO Создаем из Префаба карту
          //  var towerLevel = towerViewModel.Level;
           // var towerType = towerViewModel.ConfigId;
           viewModel.Position = new Vector2Int(row * 210 + 10 , - col * 310 - 10);
            var prefabTowerCardPath =
                $"Prefabs/UI/MainMenu/ScreenInventory/TowerCard"; 
            var towerPrefab = Resources.Load<TowerCardBinder>(prefabTowerCardPath);
            var createdTower = Instantiate(towerPrefab, containerTowerCard);
            createdTower.Bind(viewModel);
            _createdTowerCardMap[viewModel.IdTowerCard] = createdTower;
        }

        private void DestroyTowerCard(TowerCardViewModel viewModel)
        {
            if (_createdTowerCardMap.TryGetValue(viewModel.IdTowerCard, out var towerCardBinder))
            {
                Destroy(towerCardBinder.gameObject);
                _createdTowerCardMap.Remove(viewModel.IdTowerCard);
            }
        }

    private void OnEnable()
        {
            buttonTest.onClick.AddListener(OnGoToPlayButtonClicked);
        }

        private void OnDisable()
        {
            buttonTest.onClick.RemoveListener(OnGoToPlayButtonClicked);
        }

        private void OnGoToPlayButtonClicked()
        {
            ViewModel.Test();
        }
        
        private void OnDestroy()
        {
            _disposable.Dispose();
        }
    }
}