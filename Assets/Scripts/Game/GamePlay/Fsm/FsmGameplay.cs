using DI;
using Game.GamePlay.Fsm.States;
using MVVM.FSM;

namespace Game.GamePlay.Fsm
{
    public class FsmGameplay
    {
        public FsmProxy Fsm;

        public FsmGameplay(DIContainer container)
        {
            Fsm = new FsmProxy();
            
            Fsm.AddState(new FsmStateGamePause(Fsm, container));
            Fsm.AddState(new FsmStateGamePlay(Fsm, container));
            Fsm.AddState(new FsmStateSkill(Fsm, container));
            Fsm.AddState(new FsmStateBuildBegin(Fsm, container));
            Fsm.AddState(new FsmStateBuild(Fsm, container));
            Fsm.AddState(new FsmStateBuildEnd(Fsm, container));
            
            Fsm.SetState<FsmStateGamePlay>();
        }

        public void UpdateState()
        {
            Fsm?.Update();
        }
        
        
    }
}