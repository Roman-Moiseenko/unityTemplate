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

        [SerializeField] private Button buttonBlacksmith ;
        [SerializeField] private List<Transform> deckCards = new(6);
        
        private readonly Dictionary<int, TowerCardBinder> _createdTowerCardMap = new();
        private readonly Dictionary<int, TowerPlanBinder> _createdTowerPlanMap = new();

        protected override void OnBind(ScreenInventoryViewModel viewModel)
        {
            //TODO Сделать пул объектов инвентаря, не загружая. Также как Damage в ScreenGameplay
            
            var d = Disposable.CreateBuilder();
            base.OnBind(viewModel);
            foreach (var towerCardViewModel in viewModel.TowerCards)
            {
                CreateTowerCard(towerCardViewModel);
                towerCardViewModel.IsDeck.Subscribe(v =>
                {
                    ChangeParentTowerCard(towerCardViewModel);
                    UpdateHeightContainerTowerCard();
                });
            }

            viewModel.TowerCards.ObserveAdd().Subscribe(e =>
            {
                var towerCardViewModel = e.Value;
                CreateTowerCard(e.Value);
                towerCardViewModel.IsDeck.Skip(1).Subscribe(v =>
                {
                    ChangeParentTowerCard(towerCardViewModel);
                    UpdateHeightContainerTowerCard();
                });
                UpdateHeightContainerTowerCard();
            }).AddTo(ref d);
            viewModel.TowerCards.ObserveRemove().Subscribe(e =>
            {
                DestroyTowerCard(e.Value);
            }).AddTo(ref d);
            
            foreach (var towerPlanViewModel in viewModel.TowerPlans)
            {
                CreateTowerPlan(towerPlanViewModel);
            }

            UpdateHeightContainerTowerPlan();
            viewModel.TowerPlans.ObserveAdd()
                .Subscribe(e =>
                {
                    CreateTowerPlan(e.Value);
                    UpdateHeightContainerTowerPlan();
                })
                .AddTo(ref d);
            viewModel.TowerPlans.ObserveRemove()
                .Subscribe(e => DestroyTowerPlan(e.Value))
                .AddTo(ref d);
            
            _disposable = d.Build();
        }

        private void ChangeParentTowerCard(TowerCardViewModel viewModel)
        {
            if (viewModel.IsDeck.Value)
            {
                _createdTowerCardMap[viewModel.IdTowerCard].transform.SetParent(deckCards[viewModel.NumberCardDeck - 1]);
                _createdTowerCardMap[viewModel.IdTowerCard].transform.localPosition = new Vector3(0, 0, 0);
            }
            else
            {
                _createdTowerCardMap[viewModel.IdTowerCard].transform.SetParent(containerTowerCard);
            }
        }

        
        private void UpdateHeightContainerTowerCard()
        {
            var container = containerTowerCard.GetComponent<RectTransform>();
            const int blockHeight = 250;
            const int blockSpacing = 22;
            var rows = Math.Ceiling(container.childCount / 5f);
            
            container.sizeDelta = container.childCount == 0 
                ? new Vector2(1040, 0) 
                : new Vector2(1040, (float)(rows * blockHeight + (rows - 1) * blockSpacing));
        }

        private void UpdateHeightContainerTowerPlan()
        {
            var container = containerTowerPlan.GetComponent<RectTransform>();
            const int blockHeight = 240;
            const int blockSpacing = 20;
            var rows = Math.Ceiling(container.childCount / 4f);
            container.sizeDelta = container.childCount == 0 
                ? new Vector2(1040, 0) 
                : new Vector2(1040, (float)(rows * blockHeight + (rows - 1) * blockSpacing));
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
        
        private void CreateTowerPlan(TowerPlanViewModel viewModel)
                {
                    var prefabTowerPlanPath =
                        $"Prefabs/UI/MainMenu/ScreenInventory/TowerPlan";
                    var towerPrefab = Resources.Load<TowerPlanBinder>(prefabTowerPlanPath);
                    var createdTower = Instantiate(towerPrefab, containerTowerPlan);
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
        
        

        private void OnEnable()
        {
            buttonBlacksmith.onClick.AddListener(OnOpenPopupBlacksmith);
        }

        private void OnDisable()
        {
            buttonBlacksmith.onClick.RemoveListener(OnOpenPopupBlacksmith);
        }

        private void OnOpenPopupBlacksmith()
        {
            ViewModel.RequestPopupBlacksmith();
        }

        private void OnDestroy()
        {
            _disposable.Dispose();
        }
    }
}