using DI;
using MVVM.FSM;
using UnityEngine;

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
            if (next?.GetType() == typeof(FsmWarriorRepair)) return true;
            Debug.Log("Ошибка выхода");
            return false;
        }

        public override void Update()
        {
        }
    }
}