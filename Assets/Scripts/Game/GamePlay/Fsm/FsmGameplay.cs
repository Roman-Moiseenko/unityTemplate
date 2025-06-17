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
            Fsm.AddState(new FsmStateSelectSkill(Fsm, container));
            Fsm.AddState(new FsmStateSetSkill(Fsm, container));
            Fsm.AddState(new FsmStateBuildBegin(Fsm, container));
            Fsm.AddState(new FsmStateBuild(Fsm, container));
            Fsm.AddState(new FsmStateBuildEnd(Fsm, container));
            
            Fsm.SetState<FsmStateGamePlay>();
        }

        public void UpdateState()
        {
            Fsm?.Update();
        }

        public bool IsStateGamePlay()
        {
            return Fsm.StateCurrent.Value.GetType() == typeof(FsmStateGamePlay);
        }
        public bool IsStateGamePause()
        {
            return Fsm.StateCurrent.Value.GetType() == typeof(FsmStateGamePause);
        }
        
        public bool IsStateGaming()
        {
            return IsStateGamePlay() || IsStateGamePause();
        }

        public bool IsStateBuildBegin()
        {
            return Fsm.StateCurrent.Value.GetType() == typeof(FsmStateBuildBegin);
        }
        
        public bool IsStateBuild()
        {
            return Fsm.StateCurrent.Value.GetType() == typeof(FsmStateBuild);
        }
        public bool IsStateBuildEnd()
        {
            return Fsm.StateCurrent.Value.GetType() == typeof(FsmStateBuildEnd);
        }
        
        public bool IsStateBuilding()
        {
            return IsStateBuildBegin() || IsStateBuild() || IsStateBuildEnd();
        }
    }
}