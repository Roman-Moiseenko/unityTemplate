using System;
using System.Collections.Generic;
using DI;
using Game.MainMenu.Services;
using Game.MainMenu.View.ScreenInventory.HeroCards;
using Game.MainMenu.View.ScreenInventory.PopupTowerCard;
using Game.MainMenu.View.ScreenInventory.SkillCards;
using Game.MainMenu.View.ScreenInventory.SkillPlans;
using Game.MainMenu.View.ScreenInventory.TowerCards;
using Game.MainMenu.View.ScreenInventory.TowerPlans;
using Game.State;
using Game.State.Inventory;
using Game.State.Inventory.TowerCards;
using Game.State.Maps.Towers;
using Game.State.Root;
using MVVM.UI;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.MainMenu.View.ScreenInventory
{
    public class ScreenInventoryViewModel : WindowViewModel
    {
        private readonly MainMenuUIManager _uiManager;

        public override string Id => "ScreenInventory";
        public override string Path => "MainMenu/ScreenInventory/";

        //Публичные данные для Binder
        //Tower Cards
        public readonly ObservableList<TowerCardViewModel> TowerCardsDeck = new();
        public readonly ObservableList<TowerCardViewModel> TowerCardsInventory = new();
        public readonly IObservableCollection<TowerPlanViewModel> TowerPlansInventory;
        //Skill Cards
        public readonly ObservableList<SkillCardViewModel> SkillCardsDeck = new();
        public readonly ObservableList<SkillCardViewModel> SkillCardsInventory = new();
        public readonly IObservableCollection<SkillPlanViewModel> SkillPlansInventory;
        //Hero Cards
        public readonly ReactiveProperty<HeroCardViewModel> HeroCardDeck = new(null);
        
        private readonly IObservableCollection<TowerCardViewModel> towerCards;
        private readonly IObservableCollection<SkillCardViewModel> skillCards;
        private readonly IObservableCollection<HeroCardViewModel> heroCards;
        
        private readonly InventoryUIManager _inventoryUIManager;
        private Dictionary<int, IDisposable> _disposableMap = new();

        public ScreenInventoryViewModel(MainMenuUIManager uiManager, DIContainer container) : base(container)
        {
            _uiManager = uiManager;
            _inventoryUIManager = container.Resolve<InventoryUIManager>();

            var towerCardPlanService = container.Resolve<TowerCardPlanService>();
            towerCards = towerCardPlanService.AllTowerCards;
            TowerPlansInventory = towerCardPlanService.AllTowerPlans;
            TowerCardsUpload();

            var skillCardPlanService = container.Resolve<SkillCardPlanService>();
            skillCards = skillCardPlanService.AllSkillCards;
            SkillPlansInventory = skillCardPlanService.AllSkillPlans;
            SkillCardsUpload();

            var heroCardService = container.Resolve<HeroCardService>();
            heroCards = heroCardService.AllHeroCards;
            HeroCardsUpload();
        }

        
        
        /*
        public void RequestOpenPopupTowerCard(TowerCardViewModel viewModel)
        {
            _uiManager.OpenPopupTowerCard(viewModel);
        }
*/


        public void RequestPopupBlacksmith()
        {
            _inventoryUIManager.OpenPopupBlacksmithTower();
        }

        private void TowerCardsUpload()
        {
            foreach (var towerCardViewModel in towerCards)
            {
                TowerDeckSubscription(towerCardViewModel);
            }

            towerCards.ObserveAdd().Subscribe(e =>
            {
                var towerCardViewModel = e.Value;
                TowerDeckSubscription(towerCardViewModel);
            }).AddTo(ref _disposables);
            towerCards.ObserveRemove().Subscribe(e =>
            {
                var towerCardViewModel = e.Value;
                TowerDeckUnsubscription(towerCardViewModel);
            }).AddTo(ref _disposables);
        }

        private void TowerDeckSubscription(TowerCardViewModel towerCardViewModel)
        {
            var d = towerCardViewModel.IsDeck.Subscribe(x =>
            {
                if (x)
                {
                    TowerCardsDeck.Add(towerCardViewModel);
                    TowerCardsInventory.Remove(towerCardViewModel);
                }
                else
                {
                    TowerCardsDeck.Remove(towerCardViewModel);
                    TowerCardsInventory.Add(towerCardViewModel);
                }
            }).AddTo(ref _disposables);
            _disposableMap.Add(towerCardViewModel.IdTowerCard, d);
        }

        private void TowerDeckUnsubscription(TowerCardViewModel towerCardViewModel)
        {
            if (towerCardViewModel.IsDeck.CurrentValue)
            {
                TowerCardsDeck.Remove(towerCardViewModel);
            }
            else
            {
                TowerCardsInventory.Remove(towerCardViewModel);
            }

            _disposableMap[towerCardViewModel.IdTowerCard].Dispose();
            _disposableMap.Remove(towerCardViewModel.IdTowerCard);
        }

        private void HeroCardsUpload()
        {
            foreach (var heroCardViewModel in heroCards)
            {
                heroCardViewModel.IsDeck
                    .Where(x => x)
                    .Subscribe(_ => HeroCardDeck.Value = heroCardViewModel)
                    .AddTo(ref _disposables);
            }
        }

        private void SkillCardsUpload()
        {
            foreach (var skillCardViewModel in skillCards)
            {
                skillCardViewModel.IsDeck.Subscribe(x =>
                {
                    if (x)
                    {
                        SkillCardsDeck.Add(skillCardViewModel);
                        SkillCardsInventory.Remove(skillCardViewModel);
                    }
                    else
                    {
                        SkillCardsDeck.Remove(skillCardViewModel);
                        SkillCardsInventory.Add(skillCardViewModel);
                    }
                }).AddTo(ref _disposables);
            }

            skillCards.ObserveAdd().Subscribe(e =>
            {
                var skillCardViewModel = e.Value;
                if (skillCardViewModel.IsDeck.CurrentValue)
                {
                    SkillCardsDeck.Add(skillCardViewModel);
                }
                else
                {
                    SkillCardsInventory.Add(skillCardViewModel);
                }
            });
            skillCards.ObserveRemove().Subscribe(e =>
            {
                var skillCardViewModel = e.Value;
                if (skillCardViewModel.IsDeck.CurrentValue)
                {
                    SkillCardsDeck.Remove(skillCardViewModel);
                }
                else
                {
                    SkillCardsInventory.Remove(skillCardViewModel);
                }
            }).AddTo(ref _disposables);
        }
    }
}