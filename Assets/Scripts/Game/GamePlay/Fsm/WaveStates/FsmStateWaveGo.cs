using DI;
using MVVM.FSM;
using UnityEngine;

namespace Game.GamePlay.Fsm.WaveStates
{
    public class FsmStateWaveGo : FSMState
    {
        public FsmStateWaveGo(FsmProxy fsm, DIContainer container) : base(fsm, container)
        {
        }


        public override void Enter()
        {
        }

        public override bool Exit(FSMState next = null)
        {
            if (next == null) return false;
            return next.GetType() == typeof(FsmStateWaveEnd);
        }

        public override void Update() { }
    }
}