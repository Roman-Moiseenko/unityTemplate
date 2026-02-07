using DI;
using MVVM.FSM;

namespace Game.GamePlay.Fsm.WarriorStates
{
    public class FsmWarriorAttack : FSMState
    {
        public FsmWarriorAttack(FsmProxy fsm) : base(fsm)
        {
            
        }
        
        public override void Enter()
        {
            
        }

        public override bool Exit(FSMState next = null)
        {
            Params = null;
            
            return true;
        }

        public override void Update()
        {
        }
    }
}