using DI;
using Game.GamePlay.Fsm.WarriorStates;
using Game.GamePlay.View.Mobs;
using Game.State.Maps.Roads;
using MVVM.FSM;
using R3;
using UnityEngine;

namespace Game.GamePlay.Fsm
{
    public class FsmWarrior
    {
        public FsmProxy Fsm;

        public bool IsMoving;
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
            Fsm.AddState(new FsmWarriorGoToPlacement(Fsm));
            Fsm.SetState<FsmWarriorNew>();

            Fsm.StateCurrent.Subscribe(state =>
            {
                if (state.GetType() == typeof(FsmWarriorGoToMob) || state.GetType() == typeof(FsmWarriorGoToRepair) || state.GetType() == typeof(FsmWarriorGoToPlacement))
                {
                    IsMoving = true;
                }
                else
                {
                    IsMoving = false;
                }
            });
        }


        public bool IsPlacement()
        {
            return Fsm.StateCurrent.CurrentValue.GetType() == typeof(FsmWarriorGoToPlacement);
        }

        public bool IsGoToMob()
        {
            return Fsm.StateCurrent.CurrentValue.GetType() == typeof(FsmWarriorGoToMob);
        }

        public Vector3 GetPosition()
        {
            return (Vector3)Fsm.GetParamsState();
        }

        public bool IsGoToRepair()
        {
            return Fsm.StateCurrent.CurrentValue.GetType() == typeof(FsmWarriorGoToRepair);
        }

        public bool IsAwait()
        {
            return Fsm.StateCurrent.CurrentValue.GetType() == typeof(FsmWarriorAwait);
        }
        private bool IsToPlacement()
        {
            return Fsm.StateCurrent.CurrentValue.GetType() == typeof(FsmWarriorGoToPlacement);

        }
        public bool IsDead()
        {
            return Fsm.StateCurrent.CurrentValue.GetType() == typeof(FsmWarriorDead);
        }
        /**
         * Состояния при которых Warrior движется
         */
        public bool IsMovingState()
        {
            return IsGoToMob() || IsGoToRepair() || IsToPlacement();
        }


        public bool IsAttack()
        {
            return Fsm.StateCurrent.CurrentValue.GetType() == typeof(FsmWarriorAttack);
        }
    }
}