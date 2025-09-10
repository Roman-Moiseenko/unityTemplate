using DI;
using MVVM.FSM;
using UnityEngine;

namespace Game.GamePlay.Fsm.WaveStates
{
    public class FsmStateWaveWait : FSMState
    {
        public FsmStateWaveWait(FsmProxy fsm, DIContainer container) : base(fsm, container)
        {
        }

        public override void Enter()
        {
        }

        public override bool Exit(FSMState next = null)
        {
            if (next == null) return false;

            var d= next.GetType() == typeof(FsmStateWaveTimer) 
                   || next.GetType() == typeof(FsmStateWaveBegin) ;
            return d;
        }

        public override void Update()
        {
        }
    }
}