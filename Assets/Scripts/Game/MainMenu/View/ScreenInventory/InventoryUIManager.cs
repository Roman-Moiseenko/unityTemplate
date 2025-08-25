using DI;
using Game.MainMenu.View.ScreenInventory.PopupTowerCard;
using Game.MainMenu.View.ScreenInventory.TowerCards;
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
            
            subjectTowerCard.Subscribe(e =>
            {
                OpenPopupTowerCard(e);
            });
        }
        
        
        private PopupTowerCardViewModel OpenPopupTowerCard(TowerCardViewModel viewModel)
        {
            var b = new PopupTowerCardViewModel(viewModel);
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