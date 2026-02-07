using DI;
using MVVM.FSM;
using UnityEngine;

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
            if (next?.GetType() == typeof(FsmWarriorGoToPlacement)) return true;
            Debug.Log("Ошибка выхода");
            return false;
        }

        public override void Update()
        {
        }
    }
}