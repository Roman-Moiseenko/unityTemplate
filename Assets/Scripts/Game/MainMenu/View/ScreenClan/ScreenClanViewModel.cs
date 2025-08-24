using DI;
using MVVM.UI;
using R3;

namespace Game.MainMenu.View.ScreenShop
{
    public class ScreenClanViewModel : WindowViewModel
    {
        private readonly MainMenuUIManager _uiManager;
      //  private readonly Subject<Unit> _exitSceneRequest;
        public override string Id => "ScreenClan";
        public override string Path => "MainMenu/";
        
        public ScreenClanViewModel(MainMenuUIManager uiManager, DIContainer container)
        {
            _uiManager = uiManager;
       //     _exitSceneRequest = exitSceneRequest;
        }
        public void RequestGoToPlay()
        {
          //  _exitSceneRequest.OnNext(Unit.Default);
        }
    }
}