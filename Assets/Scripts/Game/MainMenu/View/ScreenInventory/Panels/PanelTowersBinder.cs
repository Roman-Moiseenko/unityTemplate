using System;
using System.Collections.Generic;
using Game.MainMenu.View.ScreenInventory.TowerCards;
using Game.MainMenu.View.ScreenInventory.TowerPlans;
using ObservableCollections;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenu.View.ScreenInventory.Panels
{
    public class PanelTowersBinder : MonoBehaviour
    {
        [SerializeField] private Button btnSort;
        [SerializeField] private Button btnBlacksmith;
        [SerializeField] private Transform containerCards;
        [SerializeField] private Transform containerPlans;

        private readonly Dictionary<int, TowerCardBinder> _createdTowerCardMap = new();
        private readonly Dictionary<int, TowerPlanBinder> _createdTowerPlanMap = new();
        private IDisposable _disposable;
        private ScreenInventoryViewModel _viewModel;

        public void Bind(ScreenInventoryViewModel viewModel)
        {
            _viewModel = viewModel;
            var d = Disposable.CreateBuilder();
            
            foreach (var towerCardViewModel in viewModel.TowerCardsInventory)
            {
                CreateTowerCard(towerCardViewModel);
            }
            
            UpdateHeightContainerTowerCard();
            viewModel.TowerCardsInventory.ObserveAdd().Subscribe(e =>
            {
               // var towerCardViewModel = e.Value;
                CreateTowerCard(e.Value);
                UpdateHeightContainerTowerCard();
            }).AddTo(ref d);
            viewModel.TowerCardsInventory.ObserveRemove().Subscribe(e =>
            {
                DestroyTowerCard(e.Value);
            }).AddTo(ref d);
            
            
            foreach (var towerPlanViewModel in viewModel.TowerPlansInventory)
            {
                CreateTowerPlan(towerPlanViewModel);
            }

            UpdateHeightContainerTowerPlan();
            viewModel.TowerPlansInventory.ObserveAdd()
                .Subscribe(e =>
                {
                    CreateTowerPlan(e.Value);
                    UpdateHeightContainerTowerPlan();
                })
                .AddTo(ref d);
            viewModel.TowerPlansInventory.ObserveRemove()
                .Subscribe(e => DestroyTowerPlan(e.Value))
                .AddTo(ref d);
            
        }


        private void CreateTowerCard(TowerCardViewModel viewModel)
        {
            var prefabTowerCardPath =
                $"Prefabs/UI/MainMenu/ScreenInventory/TowerCard";
            var towerPrefab = Resources.Load<TowerCardBinder>(prefabTowerCardPath);
            var createdTower = Instantiate(towerPrefab, containerCards);
            createdTower.Bind(viewModel);
            createdTower.transform.SetSiblingIndex(0);
            _createdTowerCardMap[viewModel.IdTowerCard] = createdTower;
        }


        private void CreateTowerPlan(TowerPlanViewModel viewModel)
        {
            var prefabTowerPlanPath =
                $"Prefabs/UI/MainMenu/ScreenInventory/TowerPlan";
            var towerPrefab = Resources.Load<TowerPlanBinder>(prefabTowerPlanPath);
            var createdTower = Instantiate(towerPrefab, containerPlans);
            createdTower.Bind(viewModel);
            _createdTowerPlanMap[viewModel.IdTowerPlan] = createdTower;
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
        private void UpdateHeightContainerTowerCard()
        {
            return;
            var container = containerCards.GetComponent<RectTransform>();
            const int blockHeight = 250;
            const int blockSpacing = 22;
            var rows = Math.Ceiling(container.childCount / 5f);
            
            container.sizeDelta = container.childCount == 0 
                ? new Vector2(1040, 0) 
                : new Vector2(1040, (float)(rows * blockHeight + (rows - 1) * blockSpacing));
        }
        private void UpdateHeightContainerTowerPlan()
        {
            return;
            var container = containerPlans.GetComponent<RectTransform>();
            const int blockHeight = 240;
            const int blockSpacing = 20;
            var rows = Math.Ceiling(container.childCount / 4f);
            container.sizeDelta = container.childCount == 0 
                ? new Vector2(1040, 0) 
                : new Vector2(1040, (float)(rows * blockHeight + (rows - 1) * blockSpacing));
        }
        private void OnEnable()
        {
            btnBlacksmith.onClick.AddListener(OnOpenPopupBlacksmith);
        }

        private void OnDisable()
        {
            btnBlacksmith.onClick.RemoveListener(OnOpenPopupBlacksmith);
        }

        private void OnOpenPopupBlacksmith()
        {
            _viewModel.RequestPopupBlacksmith();
        }

        private void OnDestroy()
        {
            _disposable.Dispose();
        }
    }
}