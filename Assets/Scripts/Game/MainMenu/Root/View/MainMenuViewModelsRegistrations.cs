using DI;
using Game.Common;
using Game.GamePlay.View.UI;
using Game.MainMenu.Services;
using Game.MainMenu.View;
using Game.MainMenu.View.ScreenInventory;
using Game.MainMenu.View.ScreenPlay;
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
            container.RegisterFactory(c => new UIMainMenuRootViewModel(container)).AsSingle();
            
            var mainMenuUIManager = new MainMenuUIManager(container);
            container.RegisterInstance(mainMenuUIManager);
            container.RegisterDisposableOnSceneExit(mainMenuUIManager);
            
            
            
            var inventoryUIManager = new InventoryUIManager(container);
            container.RegisterInstance(inventoryUIManager);
            container.RegisterDisposableOnSceneExit(inventoryUIManager);
            
            var playUIManager = new PlayUIManager(container);
            container.RegisterInstance(playUIManager);
            container.RegisterDisposableOnSceneExit(playUIManager);
            
          //  container.RegisterFactory(c => new UIMainMenuRootViewModel(container.Resolve<SomeMainMenuService>())).AsSingle();
        }
    }
}