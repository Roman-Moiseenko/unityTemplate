using DI;
using Game.GamePlay.Fsm.TowerStates;
using Game.GamePlay.View.Towers;
using MVVM.FSM;

namespace Game.GamePlay.Fsm
{
    public class FsmTower
    {
        public FsmProxy Fsm;

        public FsmTower(DIContainer container)
        {
            Fsm = new FsmProxy();
            Fsm.AddState(new FsmTowerNone(Fsm, container));
            Fsm.AddState(new FsmTowerSelected(Fsm, container));
            Fsm.AddState(new FsmTowerDelete(Fsm, container));
            Fsm.AddState(new FsmTowerPlacement(Fsm, container));
            Fsm.AddState(new FsmTowerPlacementEnd(Fsm, container));
            
            Fsm.SetState<FsmTowerNone>();
        }


        public bool IsNone()
        {
            return Fsm.StateCurrent.Value.GetType() == typeof(FsmTowerNone);
        }

        public bool IsSelected()
        {
            return Fsm.StateCurrent.Value.GetType() == typeof(FsmTowerSelected);
        }
        
        public void UpdateState()
        {
            Fsm?.Update();
        }

        public TowerViewModel GetTowerViewModel()
        {
            return (TowerViewModel)Fsm.Params; 
        }

        public bool IsPlacement()
        {
            return Fsm.StateCurrent.Value.GetType() == typeof(FsmTowerPlacement);
        }
    }
}