using DI;
using MVVM.FSM;

namespace Game.GamePlay.Fsm.SkillStates
{
    public class FsmSkillSetTarget : FSMState
    {
        public FsmSkillSetTarget(FsmProxy fsm, DIContainer container) : base(fsm, container)
        {
            
        }

        public override void Enter()
        {
            
            
        }

        public override bool Exit(FSMState next = null)
        {
            return true;
        }

        public override void Update() { }
    }
}