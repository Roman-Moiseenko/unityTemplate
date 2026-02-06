using DI;
using MVVM.FSM;

namespace Game.GamePlay.Fsm.WarriorStates
{
    public class FsmWarriorDead : FSMState
    {
        public FsmWarriorDead(FsmProxy fsm) : base(fsm)
        {
            
        }
        
        public override void Enter()
        {
            Params = null;
        }

        public override bool Exit(FSMState next = null)
        {
            return true;
        }

        public override void Update()
        {
        }
    }
}