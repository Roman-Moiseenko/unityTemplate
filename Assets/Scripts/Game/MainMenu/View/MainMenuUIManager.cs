using DI;
using Game.Common;
using Game.MainMenu.Root;
using Game.MainMenu.View.MainScreen;
using Game.MainMenu.View.ScreenInventory;
using Game.MainMenu.View.ScreenPlay;
using Game.MainMenu.View.ScreenResearch;
using Game.MainMenu.View.ScreenShop;
using MVVM.UI;
using R3;

namespace Game.MainMenu.View
{
    public class MainMenuUIManager : UIManager
    {
     //   private readonly Subject<Unit> _exitSceneRequestDefault;
        private readonly Subject<MainMenuExitParams> _exitSceneRequest;

        public MainMenuUIManager(DIContainer container) : base(container)
        {
     //       _exitSceneRequestDefault = container.Resolve<Subject<Unit>>(AppConstants.EXIT_SCENE_REQUEST_TAG);
            _exitSceneRequest = container.Resolve<Subject<MainMenuExitParams>>(AppConstants.EXIT_SCENE_REQUEST_TAG);
        }

        public MainScreenViewModel OpenMainScreen()
        {
            var viewModel = new MainScreenViewModel(this);
            var rootUI = Container.Resolve<UIMainMenuRootViewModel>();
            rootUI.OpenScreen(viewModel);
            return viewModel;
        }
        
        public ScreenShopViewModel OpenScreenShop()
        {
            var viewModel = new ScreenShopViewModel(this);
            var rootUI = Container.Resolve<UIMainMenuRootViewModel>();
            rootUI.OpenScreen(viewModel);
            return viewModel;
        }

        public ScreenInventoryViewModel OpenScreenInventory()
        {
            var viewModel = new ScreenInventoryViewModel(this);
            var rootUI = Container.Resolve<UIMainMenuRootViewModel>();
            rootUI.OpenScreen(viewModel);
            return viewModel;
        }
        
        public ScreenClanViewModel OpenScreenClan()
        {
            var viewModel = new ScreenClanViewModel(this);
            var rootUI = Container.Resolve<UIMainMenuRootViewModel>();
            rootUI.OpenScreen(viewModel);
            return viewModel;
        }
        public ScreenPlayViewModel OpenScreenPlay()
        {
            var viewModel = new ScreenPlayViewModel(this, _exitSceneRequest);
            var rootUI = Container.Resolve<UIMainMenuRootViewModel>();
            rootUI.OpenScreen(viewModel);
            return viewModel;
        }
        public ScreenResearchViewModel OpenScreenResearch()
        {
            var viewModel = new ScreenResearchViewModel(this);
            var rootUI = Container.Resolve<UIMainMenuRootViewModel>();
            rootUI.OpenScreen(viewModel);
            return viewModel;
        }
        
        
    }
}