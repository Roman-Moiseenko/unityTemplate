using DI;
using MVVM.FSM;

namespace Game.GamePlay.Fsm.TowerStates
{
    public class FsmTowerDelete : FSMState
    {
        public FsmTowerDelete(FsmProxy fsm, DIContainer container) : base(fsm, container)
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