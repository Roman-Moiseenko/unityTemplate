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
            Params = null;
        }

        public override bool Exit(FSMState next = null)
        {
            if (next?.GetType() == typeof(FsmWarriorAwait)) return true;
            Debug.Log("Ошибка выхода");
            return false;
            
        }

        public override void Update()
        {
        }
    }
}