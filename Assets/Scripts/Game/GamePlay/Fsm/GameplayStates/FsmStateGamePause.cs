using DI;
using Game.State;
using MVVM.FSM;

namespace Game.GamePlay.Fsm.GameplayStates
{
    public class FsmStateGamePause : FSMState
    {
        public FsmStateGamePause(FsmProxy fsm, DIContainer container) : base(fsm, container)
        {
            
        }

        public override void Enter()
        {
            //Ставим игру на паузу
            _container.Resolve<IGameStateProvider>().GameplayState.SetPauseGame();
        }

        public override bool Exit(FSMState next = null)
        {
            if (next == null) return false;
            if (next.GetType() != typeof(FsmStateGamePlay)) return false;
            
            _container.Resolve<IGameStateProvider>().GameplayState.GameplayReturn();
            return true;
        }

        public override void Update()
        {
            
        }
    }
}