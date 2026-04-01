using System;
using System.Collections.Generic;
using Game.MainMenu.View.ScreenInventory.Deck;
using Game.MainMenu.View.ScreenInventory.Panels;
using Game.MainMenu.View.ScreenInventory.SkillCards;
using Game.MainMenu.View.ScreenInventory.SkillPlans;
using Game.MainMenu.View.ScreenInventory.TowerCards;
using Game.MainMenu.View.ScreenInventory.TowerPlans;
using MVVM.UI;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.MainMenu.View.ScreenInventory
{
    public class ScreenInventoryBinder : WindowBinder<ScreenInventoryViewModel>
    {

        [SerializeField] private DeckBinder deckBinder;
        [SerializeField] private PanelTowersBinder panelTowersBinder;
        [SerializeField] private PanelSkillsBinder panelSkillsBinder;
        
        //    [SerializeField] private Button _btnGoToPlay;
        private IDisposable _disposable;
        [SerializeField] private Transform containerTowerCard;
        [SerializeField] private Transform containerTowerPlan;

        [SerializeField] private Transform containerSkillCard;
        [SerializeField] private Transform containerSkillPlan;
        
        [SerializeField] private Button buttonBlacksmithTower ;
        [SerializeField] private Button buttonBlacksmithSkill ;
        [SerializeField] private List<Transform> towerCards = new(6);
        [SerializeField] private List<Transform> skillCards = new(2);
        [SerializeField] private Transform heroCard;
        private readonly Dictionary<int, TowerCardBinder> _createdTowerCardMap = new();
        private readonly Dictionary<int, TowerPlanBinder> _createdTowerPlanMap = new();
        
        private readonly Dictionary<int, SkillCardBinder> _createdSkillCardMap = new();
        private readonly Dictionary<int, SkillPlanBinder> _createdSkillPlanMap = new();
        

        protected override void OnBind(ScreenInventoryViewModel viewModel)
        {
            //TODO Сделать пул объектов инвентаря, не загружая. Также как Damage в ScreenGameplay
            
            var d = Disposable.CreateBuilder();
            base.OnBind(viewModel);
            
            deckBinder.Bind(viewModel);
            panelTowersBinder.Bind(viewModel);
            panelSkillsBinder.Bind(viewModel);
            
            
            /*
            //Заполняем карточками и чертежами Башен
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
            
            //Заполняем карточками и чертежами Навыков
            */
            _disposable = d.Build();
        }

        private void ChangeParentTowerCard(TowerCardViewModel viewModel)
        {
            if (viewModel.IsDeck.Value)
            {
                _createdTowerCardMap[viewModel.IdTowerCard].transform.SetParent(towerCards[viewModel.NumberCardDeck - 1]);
                _createdTowerCardMap[viewModel.IdTowerCard].transform.localPosition = new Vector3(0, 0, 0);
            }
            else
            {
                _createdTowerCardMap[viewModel.IdTowerCard].transform.SetParent(containerTowerCard);
            }
        }

        
        private void UpdateHeightContainerTowerCard()
        {
            return;
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
            return;
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
            buttonBlacksmithTower.onClick.AddListener(OnOpenPopupBlacksmith);
        }

        private void OnDisable()
        {
            buttonBlacksmithTower.onClick.RemoveListener(OnOpenPopupBlacksmith);
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