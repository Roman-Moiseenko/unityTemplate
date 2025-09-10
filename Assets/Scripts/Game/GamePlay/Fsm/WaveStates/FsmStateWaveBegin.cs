using DI;
using MVVM.FSM;

namespace Game.GamePlay.Fsm.WaveStates
{
    public class FsmStateWaveBegin : FSMState
    {
        public FsmStateWaveBegin(FsmProxy fsm, DIContainer container) : base(fsm, container)
        {
        }
        public override void Enter()
        {
            // Debug.Log("FsmStateWaveWait Enter");
        }

        public override bool Exit(FSMState next = null)
        {
            if (next == null) return false;

            return next.GetType() == typeof(FsmStateWaveGo);
        }

        public override void Update()
        {
        }
    }
}