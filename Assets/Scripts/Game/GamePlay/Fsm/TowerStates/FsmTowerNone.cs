using DI;
using MVVM.FSM;

namespace Game.GamePlay.Fsm.TowerStates
{
    public class FsmTowerNone : FSMState
    {
        public FsmTowerNone(FsmProxy fsm, DIContainer container) : base(fsm, container)
        {
            
        }

        public override void Enter()
        {
            //При выходе из состояния Tower обнуляем параметры
            Params = null;
        }

        public override bool Exit(FSMState next = null)
        {
            return true;
        }

        public override void Update() { }
    }
}