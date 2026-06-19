using DI;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.GameplayStates;
using Game.GamePlay.Fsm.HeroStates;
using Game.GamePlay.Fsm.TowerStates;
using Game.GamePlay.Root;
using Game.GamePlay.Services;
using Game.GamePlay.View.UI.PanelActions;
using Game.GamePlay.View.UI.PanelBuild;
using Game.GamePlay.View.UI.PanelConfirmation;
using Game.GamePlay.View.UI.PanelGateWave;
using Game.GamePlay.View.UI.PanelHeroAction;
using Game.GamePlay.View.UI.PanelHeroPlacement;
using Game.GamePlay.View.UI.PanelTowerAction;
using Game.GamePlay.View.UI.PanelTowerPlacement;
using Game.GamePlay.View.UI.PopupExitNotSave;
using Game.GamePlay.View.UI.PopupFinishGameplay;
using Game.GamePlay.View.UI.PopupLose;
using Game.GamePlay.View.UI.PopupPause;
using Game.GamePlay.View.UI.PopupSettings;
using Game.GamePlay.View.UI.PopupStatistics;
using Game.GamePlay.View.UI.PopupTowerDelete;
using Game.GamePlay.View.UI.ScreenGameplay;
using Game.State;
using MVVM.UI;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.UI
{
    public class GameplayUIManager : UIManager
    {
        private readonly Subject<GameplayExitParams> _exitSceneRequest;
        private readonly FsmGameplay _fsmGameplay;
        private ScreenGameplayViewModel _screenGameplayViewModel; //Кешируем главный экран геймплея - решение не очень, переделать
        private readonly FsmTower _fsmTower;
        private readonly FsmHero _fsmHero;

        public GameplayUIManager(DIContainer container) : base(container)
        {
            var gameStateProvider = container.Resolve<IGameStateProvider>(); //Получаем репозиторий
            var rootUI = Container.Resolve<UIGameplayRootViewModel>();
            var gameService = container.Resolve<GameplayService>();            
            
            _fsmGameplay = container.Resolve<FsmGameplay>();
            _fsmTower = container.Resolve<FsmTower>();
            _fsmHero = container.Resolve<FsmHero>();
            _exitSceneRequest = container.Resolve<Subject<GameplayExitParams>>();
            
            //Создаем панели, необходимые для Геймплея
            rootUI.AddPanel(new PanelGateWaveViewModel(this, container));
            rootUI.AddPanel(new PanelBuildViewModel(container));
            rootUI.AddPanel(new PanelActionsViewModel(this, container));
            rootUI.AddPanel(new PanelConfirmationViewModel(this, container));
            rootUI.AddPanel(new PanelTowerActionViewModel(this, container));
            rootUI.AddPanel(new PanelTowerPlacementViewModel(this, container));
            //Hero Panels
            rootUI.AddPanel(new PanelHeroPlacementViewModel(this, container));
            rootUI.AddPanel(new PanelHeroActionViewModel(this, container));
            //Скрываем панель при первом вхождении в геймплей
            rootUI.HidePanel<PanelActionsViewModel>();            

            //Логика показа/скрытия панелей от состояний FSM (Gameplay и Tower)
            //Основные панели
            _fsmGameplay.Fsm.StateCurrent.Subscribe(newValue =>
            {
                if (newValue == null) return;
                if (newValue.GetType() == typeof(FsmStateBuildBegin))
                {
                    rootUI.HidePanel<PanelConfirmationViewModel>();
                    rootUI.ShowPanel<PanelBuildViewModel>();
                    
                }
                if (newValue.GetType() == typeof(FsmStateBuild))
                {
                    rootUI.HidePanel<PanelBuildViewModel>();
                    rootUI.ShowPanel<PanelConfirmationViewModel>();
                    rootUI.HidePanel<PanelActionsViewModel>();
                }
                if (newValue.GetType() == typeof(FsmStateBuildEnd))
                {
                    rootUI.HidePanel<PanelConfirmationViewModel>();
                    rootUI.HidePanel<PanelBuildViewModel>();
                    rootUI.ShowPanel<PanelActionsViewModel>();
                }
            }).AddTo(ref _disposables);
            //Панели Tower
            _fsmTower.Fsm.StateCurrent.Subscribe(newState =>
            {
                if (newState.GetType() == typeof(FsmTowerSelected))
                {
                    rootUI.HidePanel<PanelActionsViewModel>();
                    rootUI.HidePanel<PanelTowerPlacementViewModel>();
                    rootUI.ShowPanel<PanelTowerActionViewModel>();
                }

                if (newState.GetType() == typeof(FsmTowerNone))
                {
                    rootUI.HidePanel<PanelTowerActionViewModel>();
                    rootUI.HidePanel<PanelTowerPlacementViewModel>();
                    if (_fsmGameplay.IsStateGaming())
                    {
                    rootUI.ShowPanel<PanelActionsViewModel>();
                    }
                }

                if (newState.GetType() == typeof(FsmTowerPlacement))
                {
                    rootUI.HidePanel<PanelTowerActionViewModel>();
                    rootUI.ShowPanel<PanelTowerPlacementViewModel>();
                }

                if (newState.GetType() == typeof(FsmTowerPlacementEnd))
                {
                    rootUI.HidePanel<PanelTowerPlacementViewModel>();
                    if (_fsmGameplay.IsStateGaming())
                    {
                        rootUI.ShowPanel<PanelActionsViewModel>();
                    }
                }
            }).AddTo(ref _disposables);
            
            //Панели Hero
            _fsmHero.Fsm.StateCurrent.Subscribe(newState =>
            {
                if (newState.GetType() == typeof(FsmHeroSelected))
                {
                    rootUI.ShowPanel<PanelHeroActionViewModel>();
                    rootUI.HidePanel<PanelActionsViewModel>();
                }
                
                if (newState.GetType() == typeof(FsmHeroPlacement))
                {
                    rootUI.HidePanel<PanelHeroActionViewModel>();
                    rootUI.ShowPanel<PanelHeroPlacementViewModel>();
                }

                if (newState.GetType() == typeof(FsmHeroPlacementEnd))
                {
                    rootUI.HidePanel<PanelHeroPlacementViewModel>();
                    rootUI.ShowPanel<PanelActionsViewModel>();
                }
                
                if (newState.GetType() == typeof(FsmHeroUnSelected))
                {
                    rootUI.HidePanel<PanelHeroActionViewModel>();
                    rootUI.HidePanel<PanelHeroPlacementViewModel>();
                    rootUI.ShowPanel<PanelActionsViewModel>();
                }
                
            }).AddTo(ref _disposables);
            gameService.GameOver
                .Where(x => x != null)
                .Subscribe(exitParams =>
                    {
                        //Debug.Log(exitParams);
                        if (exitParams.MainMenuEnterParams == null) return;
                        _fsmGameplay.Fsm.SetState<FsmStateGamePause>();
                        OpenFinishPopup(exitParams);
                    }
                )
                .AddTo(ref _disposables);
        }

        public ScreenGameplayViewModel OpenScreenGameplay()
        {
            var viewModel = new ScreenGameplayViewModel(this, _exitSceneRequest, Container);
            var rootUI = Container.Resolve<UIGameplayRootViewModel>();
            rootUI.OpenScreen(viewModel);
            _screenGameplayViewModel = viewModel;
            return viewModel;
        }

        public PopupPauseViewModal OpenPopupPause()
        {
            var a = new PopupPauseViewModal(this, Container);
            var rootUI = Container.Resolve<UIGameplayRootViewModel>();
            _fsmGameplay.Fsm.SetState<FsmStateGamePause>();
            a.CloseRequested.Subscribe(e =>
            {
                _fsmGameplay.Fsm.SetState<FsmStateGamePlay>();
            }).AddTo(ref _disposables);
            rootUI.OpenPopup(a);
            return a;
        }

        public PopupExitNotSaveViewModel OpenPopupExitNotSave()
        {
            var a = new PopupExitNotSaveViewModel(Container);
            var rootUI = Container.Resolve<UIGameplayRootViewModel>();
            _fsmGameplay.Fsm.SetState<FsmStateGamePause>();
            a.CloseRequested.Subscribe(e =>
            {
                _fsmGameplay.Fsm.SetState<FsmStateGamePlay>();
            }).AddTo(ref _disposables);
            rootUI.OpenPopup(a);
            return a; 
        }

        public PopupFinishGameplayViewModel OpenFinishPopup(GameplayExitParams exitParams)
        {
            //TODO Закрыть все другие попап
            //TODO Закрыть все панели
            var finish = new PopupFinishGameplayViewModel(exitParams, _exitSceneRequest, Container);
            var rootUI = Container.Resolve<UIGameplayRootViewModel>();
            rootUI.CloseAllPopupHandler.OnNext(true);
            rootUI.HideAllPanelHandler.OnNext(true);
            _screenGameplayViewModel?.ShowTopMenu.OnNext(false);
            rootUI.OpenPopup(finish);
            
            return finish;
        }

        public PopupTowerDeleteViewModel OpenPopupTowerDelete()
        {
            var delete = new PopupTowerDeleteViewModel(Container);
            var rootUI = Container.Resolve<UIGameplayRootViewModel>();
            _fsmGameplay.Fsm.SetState<FsmStateGamePause>(); //Меняем состояние на Пауза
            delete.CloseRequested.Subscribe(_ =>
            {
                //При выходе возвращаем скорость игры и отключаем состояние Башня
                _fsmTower.Fsm.SetState<FsmTowerNone>();
                _fsmGameplay.Fsm.SetState<FsmStateGamePlay>();
            }).AddTo(ref _disposables);
            rootUI.OpenPopup(delete);
            return delete;
            
        }

        public PopupLoseViewModel OpenPopupLose()
        {
            var lose = new PopupLoseViewModel(this, _exitSceneRequest, Container);
            var rootUI = Container.Resolve<UIGameplayRootViewModel>();
            _fsmGameplay.Fsm.SetState<FsmStateGamePause>(); //Меняем состояние на Пауза
            lose.CloseRequested.Subscribe(e =>
            {
                _fsmGameplay.Fsm.SetState<FsmStateGamePlay>();
            }).AddTo(ref _disposables);
            rootUI.OpenPopup(lose);
            return lose;
        }

        public PopupSettingsViewModel OpenPopupSettings()
        {
            var settings = new PopupSettingsViewModel(Container);
            var rootUI = Container.Resolve<UIGameplayRootViewModel>();
            _fsmGameplay.Fsm.SetState<FsmStateGamePause>(); //Меняем состояние на Пауза
            settings.CloseRequested.Subscribe(e =>
            {
                _fsmGameplay.Fsm.SetState<FsmStateGamePlay>();
            }).AddTo(ref _disposables);
            rootUI.OpenPopup(settings);
            return settings;
        }
        
        public PopupStatisticsViewModel OpenPopupStatistics()
        {
            var statistics = new PopupStatisticsViewModel(Container);
            var rootUI = Container.Resolve<UIGameplayRootViewModel>();
            _fsmGameplay.Fsm.SetState<FsmStateGamePause>(); //Меняем состояние на Пауза
            statistics.CloseRequested.Subscribe(e =>
            {
                _fsmGameplay.Fsm.SetState<FsmStateGamePlay>();
            }).AddTo(ref _disposables);
            rootUI.OpenPopup(statistics);
            return statistics;
        }
    }
}