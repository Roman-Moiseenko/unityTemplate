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

            UpdateHeightContainerCard();
            viewModel.TowerCardsInventory.ObserveAdd().Subscribe(e =>
            {
                // var towerCardViewModel = e.Value;
                CreateTowerCard(e.Value);
                UpdateHeightContainerCard();
            }).AddTo(ref d);
            viewModel.TowerCardsInventory.ObserveRemove().Subscribe(e =>
            {
                DestroyTowerCard(e.Value);
                UpdateHeightContainerCard();
            }).AddTo(ref d);


            foreach (var towerPlanViewModel in viewModel.TowerPlansInventory)
            {
                CreateTowerPlan(towerPlanViewModel);
            }

            UpdateHeightContainerPlan();
            viewModel.TowerPlansInventory.ObserveAdd()
                .Subscribe(e =>
                {
                    CreateTowerPlan(e.Value);
                    UpdateHeightContainerPlan();
                })
                .AddTo(ref d);
            viewModel.TowerPlansInventory.ObserveRemove()
                .Subscribe(e => DestroyTowerPlan(e.Value))
                .AddTo(ref d);

            _disposable = d.Build();
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

        private void UpdateHeightContainerCard()
        {
            // return;
            var container = containerCards.GetComponent<RectTransform>();
            var sizeDelta = container.sizeDelta;
            var count = _viewModel.TowerCardsInventory.Count;
            const int blockHeight = 250;
            const int blockSpacing = 22;
            var rows = Math.Ceiling(count / 5f);

            Debug.Log(" count = " + count + ", rows = " + rows);
            sizeDelta.y = count == 0 ? 0 : (float)(rows * blockHeight + (rows - 1) * blockSpacing);
            sizeDelta.y += 40f;
            Debug.Log(" sizeDelta.y = " + sizeDelta.y);
            
            container.sizeDelta = sizeDelta;
        }

        private void UpdateHeightContainerPlan()
        {
            //return;
            var container = containerPlans.GetComponent<RectTransform>();
            var sizeDelta = container.sizeDelta;
            const int blockHeight = 240;
            const int blockSpacing = 20;
            var rows = Math.Ceiling(container.childCount / 4f);
            sizeDelta.y = container.childCount == 0 ? 0 : (float)(rows * blockHeight + (rows - 1) * blockSpacing);
            container.sizeDelta = sizeDelta;
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