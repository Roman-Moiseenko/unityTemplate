using DI;
using Game.State;
using MVVM.UI;
using R3;

namespace Game.MainMenu.View.MainScreen
{
    public class MainScreenViewModel : WindowViewModel
    {
        private readonly MainMenuUIManager _uiManager;
        private readonly DIContainer _container;
        public override string Id => "MainScreen";
        public override string Path => "MainMenu/";

        public readonly ReactiveProperty<long> SoftCurrency;
        public readonly ReactiveProperty<long> HardCurrency;
        
        //public
        public MainScreenViewModel(MainMenuUIManager uiManager, DIContainer container) : base(container)
        {
            _uiManager = uiManager;
            _container = container;
            var gameState = container.Resolve<IGameStateProvider>().GameState;
            SoftCurrency = gameState.SoftCurrency;
            HardCurrency = gameState.HardCurrency;
        }

        public void OpenShop()
        {
            _uiManager.OpenScreenShop();
        }
        
        public void OpenInventory()
        {
            _uiManager.OpenScreenInventory();
        }
        public void OpenPlay()
        {
            _uiManager.OpenScreenPlay();
        }
        public void OpenClan()
        {
            _uiManager.OpenScreenClan();
        }
        public void OpenResearch()
        {
            _uiManager.OpenScreenResearch();
        }

        public void OpenProfile()
        {
            _uiManager.OpenPopupProfile();
        }
        
    }
}