using MVVM.UI;
using R3;

namespace Game.MainMenu.View.ScreenPlay
{
    public class ScreenPlayViewModel : WindowViewModel
    {
        private readonly MainMenuUIManager _uiManager;
        private readonly Subject<Unit> _exitSceneRequest;
        public override string Id => "ScreenPlay";
        public override string Path => "MainMenu/";
        
        public ScreenPlayViewModel(MainMenuUIManager uiManager, Subject<Unit> exitSceneRequest)
        {
            _uiManager = uiManager;
            _exitSceneRequest = exitSceneRequest;
        }
        public void RequestGoToPlay()
        {
            _exitSceneRequest.OnNext(Unit.Default);
        }
    }
}