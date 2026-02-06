using DI;
using Game.GamePlay.Fsm.WarriorStates;
using Game.GamePlay.View.Mobs;
using MVVM.FSM;

namespace Game.GamePlay.Fsm
{
    public class FsmWarrior
    {
        public FsmProxy Fsm;
        public FsmWarrior()
        {
            Fsm = new FsmProxy();
            
            Fsm.AddState(new FsmWarriorNew(Fsm));
            Fsm.AddState(new FsmWarriorAttack(Fsm));
            Fsm.AddState(new FsmWarriorAwait(Fsm));
            Fsm.AddState(new FsmWarriorDead(Fsm));
            Fsm.AddState(new FsmWarriorGoToMob(Fsm));
            Fsm.AddState(new FsmWarriorGoToRepair(Fsm));
            Fsm.AddState(new FsmWarriorRepair(Fsm));
            Fsm.AddState(new FsmWarriorToPlacement(Fsm));
            
            Fsm.SetState<FsmWarriorNew>();
        }


        public bool IsPlacement()
        {
            return Fsm.StateCurrent.CurrentValue.GetType() == typeof(FsmWarriorToPlacement);
        }

        public bool IsGoToMob()
        {
            return Fsm.StateCurrent.CurrentValue.GetType() == typeof(FsmWarriorGoToMob);
        }

        public MobViewModel GetTarget()
        {
            return (MobViewModel)Fsm.Params;
        }

        public bool IsGoToRepair()
        {
            return Fsm.StateCurrent.CurrentValue.GetType() == typeof(FsmWarriorGoToRepair);
        }

        public bool IsAwait()
        {
            return Fsm.StateCurrent.CurrentValue.GetType() == typeof(FsmWarriorAwait);
        }
        public void ClearParams()
        {
            Fsm.ClearParams();
        }

    }
}