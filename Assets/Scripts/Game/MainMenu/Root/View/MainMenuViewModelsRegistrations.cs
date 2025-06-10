using DI;
using Game.Common;
using Game.GamePlay.View.UI;
using Game.MainMenu.Services;
using Game.MainMenu.View;
using Game.Settings;
using Game.State;
using R3;

namespace Game.MainMenu.Root.View
{
    
    /**
     * Регистрируем View-модели сцены
     */
    public static class MainMenuViewModelsRegistrations
    {
        public static void Register(DIContainer container)
        {
            
            container.RegisterFactory(c => new MainMenuUIManager(container)).AsSingle();
            container.RegisterFactory(c => new UIMainMenuRootViewModel(container)).AsSingle();
            
          //  container.RegisterFactory(c => new UIMainMenuRootViewModel(container.Resolve<SomeMainMenuService>())).AsSingle();
        }
    }
}