using DI;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.States;
using Game.State;
using Game.State.Root;
using MVVM.UI;
using R3;

namespace Game.GamePlay.View.UI.PanelActions
{
    public class PanelActionsViewModel : WindowViewModel
    {
        public override string Id => "PanelActions";
        public override string Path => "Gameplay/";
        
        public readonly int CurrentSpeed;
        public readonly GameplayUIManager _uiManager;
        
        private readonly GameplayState _gameplayState;
        private readonly FsmGameplay _fsmGameplay;
        public PanelActionsViewModel(
            GameplayUIManager uiManager, 
            DIContainer container
        )
        {
            _uiManager = uiManager;
            
            _gameplayState = container.Resolve<IGameStateProvider>().GameState.GameplayState;
            _fsmGameplay = container.Resolve<FsmGameplay>();
            CurrentSpeed = _gameplayState.GetCurrentSpeed();
        }
        public int RequestGameSpeed()
        {
            return _gameplayState.SetNextSpeed();
        }

        public void RequestToBuild()
        {
            _fsmGameplay.Fsm.SetState<FsmStateBuildBegin>();
        }
        
    }
}