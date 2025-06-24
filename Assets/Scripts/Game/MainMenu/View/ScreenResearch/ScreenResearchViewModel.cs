using MVVM.UI;
using R3;

namespace Game.MainMenu.View.ScreenResearch
{
    public class ScreenResearchViewModel : WindowViewModel
    {
        private readonly MainMenuUIManager _uiManager;
   //     private readonly Subject<Unit> _exitSceneRequest;
        public override string Id => "ScreenResearch";
        public override string Path => "MainMenu/";
        
        public ScreenResearchViewModel(MainMenuUIManager uiManager)
        {
            _uiManager = uiManager;
   //         _exitSceneRequest = exitSceneRequest;
        }
        public void RequestGoToPlay()
        {
        //    _exitSceneRequest.OnNext(Unit.Default);
        }
    }
}