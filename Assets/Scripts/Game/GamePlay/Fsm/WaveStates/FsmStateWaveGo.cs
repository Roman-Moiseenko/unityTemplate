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
          //  Debug.Log("FsmStateWaveGo Enter");
        }

        public override bool Exit(FSMState next = null)
        {
            if (next == null) return false;
            if (next.GetType() == typeof(FsmStateWaveEnd))
            {
            //    Debug.Log("FsmStateWaveGo Exit");
                return true;
            }

            return false;

        }

        public override void Update() { }
    }
}