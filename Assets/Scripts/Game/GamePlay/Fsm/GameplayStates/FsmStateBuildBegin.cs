using DI;
using Game.State;
using Game.State.Root;
using MVVM.FSM;

namespace Game.GamePlay.Fsm.GameplayStates
{
    public class FsmStateBuildBegin : FSMState
    {
        private int _previousGameSpeed;
        private GameplayStateProxy _gameplayStateProxy;

        public FsmStateBuildBegin(FsmProxy fsm, DIContainer container) : base(fsm, container) { }

        public override void Enter()
        {
          //  Debug.Log("FsmStateBuildBegin " + _container.Resolve<IGameStateProvider>().GameplayState.GameSpeed.Value);
            _container.Resolve<IGameStateProvider>().GameplayState.SetPauseGame();
            //_gameplayStateProxy.SetPauseGame();
        }

        public override bool Exit(FSMState next = null)
        {
            if (next == null) return false;
            return next.GetType() == typeof(FsmStateBuild) || next.GetType() == typeof(FsmStateBuildEnd);
        }

        public override void Update() { }
    }
}