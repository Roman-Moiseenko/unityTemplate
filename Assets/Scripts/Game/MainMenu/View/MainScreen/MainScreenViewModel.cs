using MVVM.UI;
using R3;

namespace Game.MainMenu.View.MainScreen
{
    public class MainScreenViewModel : WindowViewModel
    {
        private readonly MainMenuUIManager _uiManager;
        public override string Id => "MainScreen";
        public override string Path => "MainMenu/";
        
        public MainScreenViewModel(MainMenuUIManager uiManager)
        {
            _uiManager = uiManager;
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
        
    }
}