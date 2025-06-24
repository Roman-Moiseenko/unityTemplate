using MVVM.UI;
using R3;

namespace Game.MainMenu.View.ScreenInventory
{
    public class ScreenInventoryViewModel : WindowViewModel
    {
        private readonly MainMenuUIManager _uiManager;
       // private readonly Subject<Unit> _exitSceneRequest;
        public override string Id => "ScreenInventory";
        public override string Path => "MainMenu/";
        
        public ScreenInventoryViewModel(MainMenuUIManager uiManager)
        {
            _uiManager = uiManager;
       //     _exitSceneRequest = exitSceneRequest;
        }
        public void RequestGoToPlay()
        {
       //     _exitSceneRequest.OnNext(Unit.Default);
        }
    }
}