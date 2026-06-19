using DI;
using Game.GamePlay.Fsm.TowerStates;
using MVVM.FSM;

namespace Game.GamePlay.Fsm.HeroStates
{
    public class FsmHeroSelected : FSMState
    {
        public FsmHeroSelected(FsmProxy fsm, DIContainer container) : base(fsm, container)
        {
            
        }

        public override void Enter()
        {
            var fsmTower = _container.Resolve<FsmTower>();
            if (fsmTower.IsSelected()) fsmTower.Fsm.SetState<FsmTowerNone>();
        }

        public override bool Exit(FSMState next = null)
        {
            return true;
        }

        public override void Update() { }
    }
}