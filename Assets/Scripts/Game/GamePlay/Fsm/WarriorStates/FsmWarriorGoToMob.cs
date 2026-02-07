using DI;
using MVVM.FSM;
using UnityEngine;

namespace Game.GamePlay.Fsm.WarriorStates
{
    public class FsmWarriorGoToMob : FSMState
    {
        public FsmWarriorGoToMob(FsmProxy fsm) : base(fsm)
        {
            
        }
        
        public override void Enter()
        {
            
        }

        public override bool Exit(FSMState next = null)
        {
            return true;
        }

        public override void Update()
        {
        }
    }
}