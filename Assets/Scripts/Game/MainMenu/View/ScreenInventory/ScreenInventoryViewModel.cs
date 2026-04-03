using System;
using System.Collections.Generic;
using DI;
using Game.MainMenu.Services;
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
        
        // private readonly Subject<Unit> _exitSceneRequest;
        public override string Id => "ScreenInventory";
        public override string Path => "MainMenu/ScreenInventory/";
        //public readonly ObservableList<TowerCardViewModel> TowerCards;

        public GameStateProxy GameState;

        private readonly IObservableCollection<TowerCardViewModel> towerCards;
        
        public readonly ObservableList<TowerCardViewModel> TowerCardsDeck = new();
        public readonly ObservableList<TowerCardViewModel> TowerCardsInventory = new();
        
        public readonly IObservableCollection<TowerPlanViewModel> TowerPlansInventory;
        
        private readonly IObservableCollection<SkillCardViewModel> skillCards;
        
        public readonly ObservableList<SkillCardViewModel> SkillCardsDeck = new();
        public readonly ObservableList<SkillCardViewModel> SkillCardsInventory = new();
        
        public IObservableCollection<SkillPlanViewModel> SkillPlansInventory;

        private readonly InventoryUIManager _inventoryUIManager;
        private Dictionary<int, IDisposable> _disposableMap = new();

        public ScreenInventoryViewModel(MainMenuUIManager uiManager, DIContainer container) : base(container)
        {
            _uiManager = uiManager;
            GameState = container.Resolve<IGameStateProvider>().GameState;
            _inventoryUIManager = container.Resolve<InventoryUIManager>();
            
            
            var towerCardPlanService = container.Resolve<TowerCardPlanService>();
            towerCards = towerCardPlanService.AllTowerCards;
            TowerPlansInventory = towerCardPlanService.AllTowerPlans;
            TowerCardsUpload();

            
            
            
            var skillCardPlanService = container.Resolve<SkillCardPlanService>();
            skillCards = skillCardPlanService.AllSkillCards;
            SkillPlansInventory = skillCardPlanService.AllSkillPlans;
            SkillCardsUpload();
            
            

/*
            foreach (var inventoryItem in GameState.InventoryItems)
            {
                if (inventoryItem is TowerCard towerCardEntity)
                {
                    var towerCardViewModel = new TowerCardViewModel(towerCardEntity);
                    TowerCards.Add(towerCardViewModel);
                }
            }
            */
            //     _exitSceneRequest = exitSceneRequest;
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
            foreach (var towerCardViewModel  in towerCards)
            {
                TowerDeckSubscription(towerCardViewModel);
            }

            towerCards.ObserveAdd().Subscribe(e =>
            {
                var towerCardViewModel = e.Value;
                TowerDeckSubscription(towerCardViewModel);
            });
            towerCards.ObserveRemove().Subscribe(e =>
            {
                var towerCardViewModel = e.Value;
                TowerDeckUnsubscription(towerCardViewModel);
            });
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
            });
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
        
        private void SkillCardsUpload()
        {
            foreach (var skillCardViewModel  in skillCards)
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
                });
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
            });
        }
    }
}