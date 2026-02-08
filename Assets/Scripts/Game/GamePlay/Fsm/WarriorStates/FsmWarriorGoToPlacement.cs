using DI;
using MVVM.FSM;
using UnityEngine;

namespace Game.GamePlay.Fsm.WarriorStates
{
    public class FsmWarriorGoToPlacement : FSMState
    {
        public FsmWarriorGoToPlacement(FsmProxy fsm) : base(fsm)
        {
            
        }
        
        public override void Enter()
        {
        }

        public override bool Exit(FSMState next = null)
        {
      //      if (next?.GetType() == typeof(FsmWarriorAwait) || next?.GetType() == typeof(FsmWarriorGoToPlacement)) return true;
//            Debug.Log("Ошибка выхода");
            return true;
            
        }

        public override void Update()
        {
        }
    }
}