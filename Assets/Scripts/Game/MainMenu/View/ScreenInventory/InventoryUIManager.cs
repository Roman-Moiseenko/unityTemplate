using DI;
using Game.MainMenu.View.ScreenInventory.PopupTowerCard;
using Game.MainMenu.View.ScreenInventory.PopupTowerPlan;
using Game.MainMenu.View.ScreenInventory.TowerCards;
using Game.MainMenu.View.ScreenInventory.TowerPlans;
using MVVM.UI;
using R3;
using UnityEngine;

namespace Game.MainMenu.View.ScreenInventory
{
    public class InventoryUIManager : UIManager
    {
        public InventoryUIManager(DIContainer container) : base(container)
        {
            var subjectTowerCard = Container.Resolve<Subject<TowerCardViewModel>>();
            var subjectTowerPlan = Container.Resolve<Subject<TowerPlanViewModel>>();
            
            subjectTowerCard.Subscribe(e => OpenPopupTowerCard(e));
            subjectTowerPlan.Subscribe(e => OpenPopupTowerPlan(e));
        }
        
        
        private PopupTowerCardViewModel OpenPopupTowerCard(TowerCardViewModel viewModel)
        {
            var b = new PopupTowerCardViewModel(viewModel, Container);
            var rootUI = Container.Resolve<UIMainMenuRootViewModel>();
            
            b.CloseRequested.Subscribe(e =>
            {
                //_fsmGameplay.Fsm.SetState<FsmStateGamePlay>();
            });
            rootUI.OpenPopup(b);
            return b;
        }
        
        
        private PopupTowerPlanViewModel OpenPopupTowerPlan(TowerPlanViewModel viewModel)
        {
            var b = new PopupTowerPlanViewModel(viewModel);
            var rootUI = Container.Resolve<UIMainMenuRootViewModel>();
            
            b.CloseRequested.Subscribe(e =>
            {
                //_fsmGameplay.Fsm.SetState<FsmStateGamePlay>();
            });
            rootUI.OpenPopup(b);
            return b;
        }
        
    }
}