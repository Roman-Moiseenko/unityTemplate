using DI;
using MVVM.FSM;

namespace Game.GamePlay.Fsm.WarriorStates
{
    public class FsmWarriorAwait : FSMState
    {
        public FsmWarriorAwait(FsmProxy fsm) : base(fsm)
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