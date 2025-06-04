using DI;
using Game.Common;
using Game.GamePlay.FSM.Play;
using Game.GamePlay.FSM.Play.States;
using Game.GamePlay.View.UI.PopupA;
using Game.GamePlay.View.UI.PopupB;
using Game.GamePlay.View.UI.ScreenGameplay;
using Game.State;
using Game.State.Root;
using MVVM.UI;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.UI
{
    public class GameplayUIManager : UIManager
    {
        private readonly Subject<Unit> _exitSceneRequest;
        private readonly Subject<FSMGamePause> _setGamePause;
        private readonly Subject<FSMGamePause> _setGameReturn;
        private readonly FSMGameplayBinder _fsmGameplayBinder;

        private readonly GameStateProxy _gameState;
      //  public readonly DIContainer Container;

        public GameplayUIManager(DIContainer container) : base(container)
        {
            var gameStateProvider = container.Resolve<IGameStateProvider>(); //Получаем репозиторий
            _gameState = gameStateProvider.GameState;
            _exitSceneRequest = container.Resolve<Subject<Unit>>(AppConstants.EXIT_SCENE_REQUEST_TAG);
        }

        public ScreenGameplayViewModel OpenScreenGameplay()
        {
            var viewModel = new ScreenGameplayViewModel(this, _exitSceneRequest, Container);
            var rootUI = Container.Resolve<UIGameplayRootViewModel>();
            rootUI.OpenScreen(viewModel);
            return viewModel;
        }

        public PopupAViewModal OpenPopupA()
        {
            var a = new PopupAViewModal();
            var rootUI = Container.Resolve<UIGameplayRootViewModel>();
            _gameState.GameplayState.SetPauseGame();// GameSpeed.Value = 0; //Ставим на паузу
            a.CloseRequested.Subscribe(e =>
            {
                _gameState.GameplayState.GameplayReturn();
            });
            rootUI.OpenPopup(a);
            return a;
        }
        
        public PopupBViewModal OpenPopupB()
        {
            var b = new PopupBViewModal();
            var rootUI = Container.Resolve<UIGameplayRootViewModel>();
            _gameState.GameplayState.SetPauseGame();// GameSpeed.Value = 0; //Ставим на паузу
            b.CloseRequested.Subscribe(e =>
            {
                _gameState.GameplayState.GameplayReturn();

            });
            rootUI.OpenPopup(b);
            return b;
        }
    }
}