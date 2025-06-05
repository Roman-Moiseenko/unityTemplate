using DI;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.States;
using Game.Settings;
using Game.State;
using Game.State.Root;
using MVVM.UI;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.UI.ScreenGameplay
{
    public class ScreenGameplayViewModel : WindowViewModel
    {
        public readonly GameplayUIManager _uiManager;
        private readonly Subject<Unit> _exitSceneRequest;
        private readonly GameplayState _gameplayState;
        private readonly FsmGameplay _fsmGameplay;
        public override string Id => "ScreenGameplay";
        public override string Path => "Gameplay/";
        public readonly int CurrentSpeed;
        public ScreenGameplayViewModel(
            GameplayUIManager uiManager, 
            Subject<Unit> exitSceneRequest,
            DIContainer container
            )
        {
            _uiManager = uiManager;
            _exitSceneRequest = exitSceneRequest;
            _gameplayState = container.Resolve<IGameStateProvider>().GameState.GameplayState;
            _fsmGameplay = container.Resolve<FsmGameplay>();
            CurrentSpeed = _gameplayState.GetCurrentSpeed();
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

        public int RequestGameSpeed()
        {
            return _gameplayState.SetNextSpeed();
        }

        public void RequestToBuild()
        {
            _fsmGameplay.Fsm.SetState<FsmStateBuild>();
        }
    }
}