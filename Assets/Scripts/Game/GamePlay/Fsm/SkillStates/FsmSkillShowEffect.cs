using DI;
using MVVM.FSM;

namespace Game.GamePlay.Fsm.SkillStates
{
    public class FsmSkillShowEffect : FSMState
    {
        public FsmSkillShowEffect(FsmProxy fsm, DIContainer container) : base(fsm, container)
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