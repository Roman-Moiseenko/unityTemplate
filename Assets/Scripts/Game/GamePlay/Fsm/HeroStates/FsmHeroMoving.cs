using DI;
using MVVM.FSM;

namespace Game.GamePlay.Fsm.HeroStates
{
    public class FsmHeroMoving : FSMState
    {
        public FsmHeroMoving(FsmProxy fsm) : base(fsm)
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