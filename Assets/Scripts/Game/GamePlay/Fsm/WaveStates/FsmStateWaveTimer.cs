using DI;
using MVVM.FSM;
using UnityEngine;

namespace Game.GamePlay.Fsm.WaveStates
{
    public class FsmStateWaveTimer : FSMState
    {
        public FsmStateWaveTimer(FsmProxy fsm, DIContainer container) : base(fsm, container)
        {
        }

        public override void Enter()
        {
            
          //  Debug.Log("FsmStateWaveTimer Enter");
        }

        public override bool Exit(FSMState next = null)
        {
            if (next == null) return false;
            //Debug.Log("FsmStateWaveTimer Exit");
            return next.GetType() == typeof(FsmStateWaveBegin);
        }

        public override void Update() { }
    }
}