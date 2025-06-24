using DI;
using Game.State;
using MVVM.FSM;

namespace Game.GamePlay.Fsm.States
{
    public class FsmStateGamePlay : FSMState
    {
        public FsmStateGamePlay(FsmProxy fsm, DIContainer container) : base(fsm, container)
        {
        }

        public override void Enter()
        {
            if (Fsm.PreviousState != null)
            {
                _container.Resolve<IGameStateProvider>().GameplayState.GameplayReturn();
            }
            
        }

        public override bool Exit(FSMState _next)
        {
            if (_next.GetType() == typeof(FsmStateGamePause)) return true;
            if (_next.GetType() == typeof(FsmStateSelectSkill)) return true;
            if (_next.GetType() == typeof(FsmStateBuildBegin)) return true;
            
            return false;
        }

        public override void Update()
        {
            
        }
    }
}