using DI;
using Game.Common;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.States;
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

        //private readonly GameStateProxy _gameState;

        private readonly FsmGameplay _fsmGameplay;
      //  public readonly DIContainer Container;

        public GameplayUIManager(DIContainer container) : base(container)
        {
            var gameStateProvider = container.Resolve<IGameStateProvider>(); //Получаем репозиторий

            _fsmGameplay = container.Resolve<FsmGameplay>();
            //_gameState = gameStateProvider.GameState;
            _exitSceneRequest = container.Resolve<Subject<Unit>>(AppConstants.EXIT_SCENE_REQUEST_TAG);

            _fsmGameplay.Fsm.StateCurrent.Subscribe(newValue =>
            {
                if (newValue == null) return;
                var rootUI = Container.Resolve<UIGameplayRootViewModel>();
                var viewModel = rootUI.OpenedScreen.CurrentValue;
                if (newValue.GetType() == typeof(FsmStateBuildBegin))
                {
                    Debug.Log("Прячем окно Action");
                    Debug.Log("Показываем Окно Build");
                    //Прячем окно Action
                    //Показываем Окно Build
                }

                if (newValue.GetType() == typeof(FsmStateBuild))
                {
                    //Прячем Окно Build
                }

                if (newValue.GetType() == typeof(FsmStateBuild))
                {
                    //Прячем Окно Build
                    //Показываем окно Action
                }
                
            });
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
            _fsmGameplay.Fsm.SetState<FsmStateGamePause>();
            a.CloseRequested.Subscribe(e =>
            {
                _fsmGameplay.Fsm.SetState<FsmStateGamePlay>();
            });
            rootUI.OpenPopup(a);
            return a;
        }
        
        public PopupBViewModal OpenPopupB()
        {
            var b = new PopupBViewModal();
            var rootUI = Container.Resolve<UIGameplayRootViewModel>();
            _fsmGameplay.Fsm.SetState<FsmStateGamePause>(); //Меняем состояние на Пауза
            b.CloseRequested.Subscribe(e =>
            {
                _fsmGameplay.Fsm.SetState<FsmStateGamePlay>();
            });
            rootUI.OpenPopup(b);
            return b;
        }

        /**
         * Показываем/Скрывем панель Action
         */
        public void ViewActionPanel(bool isVisible)
        {
            //
        }
        /**
         * Показываем/Скрывем панель Card
         */
        public void ViewCardPanel(bool isVisible)
        {
            //
        }

        /**
         * Панель подтверждения действия Строить/Отмена
         */
        public void ViewConfirmPanel(bool isVisible)
        {
            
        }
        
        
    }
}