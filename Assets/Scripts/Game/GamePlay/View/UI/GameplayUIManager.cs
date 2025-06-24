using DI;
using Game.Common;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.States;
using Game.GamePlay.Root;
using Game.GamePlay.View.UI.PanelActions;
using Game.GamePlay.View.UI.PanelBuild;
using Game.GamePlay.View.UI.PanelConfirmation;
using Game.GamePlay.View.UI.PopupB;
using Game.GamePlay.View.UI.PopupPause;
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
        private readonly Subject<GameplayExitParams> _exitSceneRequest;
        private readonly FsmGameplay _fsmGameplay;

        public GameplayUIManager(DIContainer container) : base(container)
        {
            var gameStateProvider = container.Resolve<IGameStateProvider>(); //Получаем репозиторий

            _fsmGameplay = container.Resolve<FsmGameplay>();
            
            var rootUI = Container.Resolve<UIGameplayRootViewModel>();
 
            //Создаем панели, необходимые для Геймплея           
            rootUI.AddPanel(new PanelBuildViewModel(container));
            rootUI.AddPanel(new PanelActionsViewModel(this, container));
            rootUI.AddPanel(new PanelConfirmationViewModel(this, container));
            
            //_gameState = gameStateProvider.GameState;
            _exitSceneRequest = container.Resolve<Subject<GameplayExitParams>>();

            _fsmGameplay.Fsm.StateCurrent.Subscribe(newValue =>
            {
                if (newValue == null) return;
                if (newValue.GetType() == typeof(FsmStateBuildBegin))
                {
                    rootUI.HidePanel<PanelConfirmationViewModel>();
                    rootUI.ShowPanel<PanelBuildViewModel>();
                    rootUI.HidePanel<PanelActionsViewModel>();
                }
                if (newValue.GetType() == typeof(FsmStateBuild))
                {
                    rootUI.HidePanel<PanelBuildViewModel>();
                    rootUI.ShowPanel<PanelConfirmationViewModel>();
                }
                if (newValue.GetType() == typeof(FsmStateBuildEnd))
                {
                    rootUI.HidePanel<PanelConfirmationViewModel>();
                    rootUI.HidePanel<PanelBuildViewModel>();
                    rootUI.ShowPanel<PanelActionsViewModel>();
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

        public PopupPauseViewModal OpenPopupPause()
        {
            var a = new PopupPauseViewModal(this, _exitSceneRequest, Container);
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