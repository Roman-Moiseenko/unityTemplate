using System.Collections.Generic;
using DI;
using Game.MainMenu.Root;
using Game.MainMenu.View.ScreenPlay.PopupFinishGameplay;
using Game.MainMenu.View.ScreenPlay.PopupOpenChest;
using Game.MainMenu.View.ScreenPlay.PopupProfile;
using Game.MainMenu.View.ScreenPlay.PopupRewardChest;
using Game.State.Inventory;
using Game.State.Inventory.Chests;
using Game.State.Maps.Rewards;
using MVVM.UI;
using R3;

namespace Game.MainMenu.View.ScreenPlay
{
    public class PlayUIManager : UIManager
    {
        public PlayUIManager(DIContainer container) : base(container)
        {
        }


        public PopupFinishGameplayViewModel OpenPopupFinishGameplay(MainMenuEnterParams enterParams)
        {
            var b = new PopupFinishGameplayViewModel(enterParams, Container);
            var rootUI = Container.Resolve<UIMainMenuRootViewModel>();
            
            b.CloseRequested.Subscribe(e =>
            {
            });
            rootUI.OpenPopup(b);
            return b;
        }
        
        public PopupOpenChestViewModel OpenPopupOpenChest(Chest chest)
        {
            var b = new PopupOpenChestViewModel(chest, Container);
            var rootUI = Container.Resolve<UIMainMenuRootViewModel>();
            
            b.CloseRequested.Subscribe(e =>
            {
            });
            rootUI.OpenPopup(b);
            return b;
        }

        public PopupRewardChestViewModel OpenPopupRewardChest(TypeChest typeChest, List<RewardEntityData> rewards)
        {
            //TODO
            
            var b = new PopupRewardChestViewModel(typeChest, rewards, Container);
            var rootUI = Container.Resolve<UIMainMenuRootViewModel>();
            
            b.CloseRequested.Subscribe(e =>
            {
            });
            rootUI.OpenPopup(b);
            return b;
            
        }
        

    }
}