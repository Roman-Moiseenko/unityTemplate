using MVVM.UI;
using R3;

namespace Game.GamePlay.View.UI.ScreenGameplay
{
    public class ScreenGameplayViewModel : WindowViewModel
    {
        public readonly GameplayUIManager _uiManager;
        private readonly Subject<Unit> _exitSceneRequest;
        public override string Id => "ScreenGameplay";
        public override string Path => "";
        public ScreenGameplayViewModel(GameplayUIManager uiManager, Subject<Unit> exitSceneRequest)
        {
            _uiManager = uiManager;
            _exitSceneRequest = exitSceneRequest;
            
        }
        
        public void RequestOpenPopupA()
        {
            _uiManager.OpenPopupA();
        }
        
        public void RequestOpenPopupB()
        {
            _uiManager.OpenPopupB();
        }

        public void RequestGoToMainMenu()
        {
            _exitSceneRequest.OnNext(Unit.Default); //Вызываем сигнал для смены сцены
        }

    }
}