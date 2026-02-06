using DI;
using MVVM.FSM;

namespace Game.GamePlay.Fsm.WarriorStates
{
    public class FsmWarriorGoToRepair : FSMState
    {
        public FsmWarriorGoToRepair(FsmProxy fsm) : base(fsm)
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