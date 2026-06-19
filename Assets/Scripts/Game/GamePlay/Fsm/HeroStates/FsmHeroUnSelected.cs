using DI;
using MVVM.FSM;

namespace Game.GamePlay.Fsm.HeroStates
{
    public class FsmHeroUnSelected : FSMState
    {
        private readonly FsmProxy _fsm;

        public FsmHeroUnSelected(FsmProxy fsm) : base(fsm)
        {
            _fsm = fsm;
        }

        public override void Enter()
        {
            _fsm.SetState<FsmHeroAwait>();
        }

        public override bool Exit(FSMState next = null)
        {
            return true;
        }

        public override void Update() { }
    }
}