using DI;
using MVVM.FSM;

namespace Game.GamePlay.Fsm.SkillStates
{
    public class FsmSkillNone : FSMState
    {
        private readonly FsmProxy _fsm;

        public FsmSkillNone(FsmProxy fsm, DIContainer container) : base(fsm, container)
        {
            _fsm = fsm;
        }

        public override void Enter()
        {
            _fsm.ClearParam();
        }

        public override bool Exit(FSMState next = null)
        {
            return true;
        }

        public override void Update() { }
    }
}