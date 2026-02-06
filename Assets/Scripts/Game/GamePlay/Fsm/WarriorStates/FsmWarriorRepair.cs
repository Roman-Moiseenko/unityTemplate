using DI;
using MVVM.FSM;

namespace Game.GamePlay.Fsm.WarriorStates
{
    public class FsmWarriorRepair : FSMState
    {
        public FsmWarriorRepair(FsmProxy fsm) : base(fsm)
        {
            
        }
        
        public override void Enter()
        {
            
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