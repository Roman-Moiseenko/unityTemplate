using DI;
using Game.GamePlay.Fsm.SkillStates;
using MVVM.FSM;

namespace Game.GamePlay.Fsm
{
    public class FsmSkill
    {
        public FsmProxy Fsm;

        public FsmSkill(DIContainer container)
        {
            Fsm = new FsmProxy();
            Fsm.AddState(new FsmSkillNone(Fsm, container));
            Fsm.AddState(new FsmSkillBegin(Fsm, container));
            Fsm.AddState(new FsmSkillSetTarget(Fsm, container));
            Fsm.AddState(new FsmSkillShowEffect(Fsm, container));
            Fsm.AddState(new FsmSkillEnd(Fsm, container));
            
            Fsm.SetState<FsmSkillNone>();
        }
    }
}