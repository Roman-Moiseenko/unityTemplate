using System.Collections.Generic;
using DI;
using Game.MainMenu.Services;
using Game.MainMenu.View.ScreenInventory.PopupTowerCard;
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

        private readonly DIContainer _container;

        // private readonly Subject<Unit> _exitSceneRequest;
        public override string Id => "ScreenInventory";
        public override string Path => "MainMenu/ScreenInventory/";
        //public readonly ObservableList<TowerCardViewModel> TowerCards;

        public GameStateProxy GameState;

        public IObservableCollection<TowerCardViewModel> TowerCards;
        public IObservableCollection<TowerPlanViewModel> TowerPlans;
        private readonly TowerCardPlanService _towerCardPlanService;
        private readonly InventoryUIManager _inventoryUIManager;

        public ScreenInventoryViewModel(MainMenuUIManager uiManager, DIContainer container)
        {
            _uiManager = uiManager;
            _container = container;
            GameState = container.Resolve<IGameStateProvider>().GameState;
            _towerCardPlanService = container.Resolve<TowerCardPlanService>();
            TowerCards = _towerCardPlanService.AllTowerCards;
            TowerPlans = _towerCardPlanService.AllTowerPlans;
            _inventoryUIManager = container.Resolve<InventoryUIManager>();

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
            _inventoryUIManager.OpenPopupBlacksmith();
        }
    }
}