using DI;
using Game.MainMenu.Root;
using Game.MainMenu.View.ScreenPlay.PopupFinishGameplay;
using Game.MainMenu.View.ScreenPlay.PopupOpenChest;
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
        
        public PopupOpenChestViewModel OpenPopupOpenChest()
        {
            var b = new PopupOpenChestViewModel(Container);
            var rootUI = Container.Resolve<UIMainMenuRootViewModel>();
            
            b.CloseRequested.Subscribe(e =>
            {
            });
            rootUI.OpenPopup(b);
            return b;
        }
    }
}