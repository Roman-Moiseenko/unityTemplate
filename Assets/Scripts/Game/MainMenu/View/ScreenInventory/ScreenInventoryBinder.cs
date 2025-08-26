using System;
using System.Collections.Generic;
using Game.MainMenu.View.ScreenInventory.TowerCards;
using Game.MainMenu.View.ScreenInventory.TowerPlans;
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
        [SerializeField] private Transform containerTowerPlan;

        [SerializeField] private Button buttonTest;

        private readonly Dictionary<int, TowerCardBinder> _createdTowerCardMap = new();
        private readonly Dictionary<int, TowerPlanBinder> _createdTowerPlanMap = new();

        protected override void OnBind(ScreenInventoryViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            base.OnBind(viewModel);
            foreach (var towerCardViewModel in viewModel.TowerCards)
            {
                CreateTowerCard(towerCardViewModel);
            }

            viewModel.TowerCards.ObserveAdd().Subscribe(e => { CreateTowerCard(e.Value); }).AddTo(ref d);
            viewModel.TowerCards.ObserveRemove().Subscribe(e =>
            {
                DestroyTowerCard(e.Value);
            }).AddTo(ref d);
            
            foreach (var towerPlanViewModel in viewModel.TowerPlans)
            {
                CreateTowerPlan(towerPlanViewModel);
            }

            viewModel.TowerPlans.ObserveAdd().Subscribe(e => { CreateTowerPlan(e.Value); }).AddTo(ref d);
            viewModel.TowerPlans.ObserveRemove().Subscribe(e =>
            {
                DestroyTowerPlan(e.Value);
            }).AddTo(ref d);
            
            
            _disposable = d.Build();
        }

        private void CreateTowerPlan(TowerPlanViewModel viewModel)
        {
            var prefabTowerPlanPath =
                $"Prefabs/UI/MainMenu/ScreenInventory/TowerPlan";
            var towerPrefab = Resources.Load<TowerPlanBinder>(prefabTowerPlanPath);
            var createdTower = Instantiate(towerPrefab, containerTowerPlan);
            createdTower.Bind(viewModel);
            _createdTowerPlanMap[viewModel.IdTowerPlan] = createdTower;
        }


        private void CreateTowerCard(TowerCardViewModel viewModel)
        {
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
        
        private void DestroyTowerPlan(TowerPlanViewModel viewModel)
        {
            if (_createdTowerPlanMap.TryGetValue(viewModel.IdTowerPlan, out var towerPlanBinder))
            {
                Destroy(towerPlanBinder.gameObject);
                _createdTowerPlanMap.Remove(viewModel.IdTowerPlan);
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