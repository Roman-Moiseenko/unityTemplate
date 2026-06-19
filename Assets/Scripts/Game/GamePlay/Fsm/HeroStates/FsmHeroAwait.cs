using DI;
using MVVM.FSM;

namespace Game.GamePlay.Fsm.HeroStates
{
    public class FsmHeroAwait : FSMState
    {
        public FsmHeroAwait(FsmProxy fsm) : base(fsm)
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