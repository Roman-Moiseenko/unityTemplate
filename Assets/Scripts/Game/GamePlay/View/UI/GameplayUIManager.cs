using DI;
using Game.Common;
using Game.GamePlay.View.UI.PopupA;
using Game.GamePlay.View.UI.PopupB;
using Game.GamePlay.View.UI.ScreenGameplay;
using MVVM.UI;
using R3;

namespace Game.GamePlay.View.UI
{
    public class GameplayUIManager : UIManager
    {
        private readonly Subject<Unit> _exitSceneRequest;

        public GameplayUIManager(DIContainer container) : base(container)
        {
            _exitSceneRequest = container.Resolve<Subject<Unit>>(AppConstants.EXIT_SCENE_REQUEST_TAG);
        }

        public ScreenGameplayViewModel OpenScreenGameplay()
        {
            var viewModel = new ScreenGameplayViewModel(this, _exitSceneRequest);
            var rootUI = Container.Resolve<UIGameplayRootViewModel>();
            rootUI.OpenScreen(viewModel);
            return viewModel;
        }

        public PopupAViewModal OpenPopupA()
        {
            var a = new PopupAViewModal();
            var rootUI = Container.Resolve<UIGameplayRootViewModel>();
            rootUI.OpenPopup(a);
            return a;
        }
        
        public PopupBViewModal OpenPopupB()
        {
            var b = new PopupBViewModal();
            var rootUI = Container.Resolve<UIGameplayRootViewModel>();
            rootUI.OpenPopup(b);
            return b;
        }
    }
}