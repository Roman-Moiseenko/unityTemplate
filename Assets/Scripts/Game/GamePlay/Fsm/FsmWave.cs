using DI;
using Game.GamePlay.Fsm.WaveStates;
using MVVM.FSM;

namespace Game.GamePlay.Fsm
{
    public class FsmWave
    {
        public FsmProxy Fsm;
        
        public FsmWave(DIContainer container)
        {
            Fsm = new FsmProxy();
            
            Fsm.AddState(new FsmStateWaveWait(Fsm, container));
            Fsm.AddState(new FsmStateWaveTimer(Fsm, container));
            Fsm.AddState(new FsmStateWaveBegin(Fsm, container));
            Fsm.AddState(new FsmStateWaveGo(Fsm, container));
            Fsm.AddState(new FsmStateWaveEnd(Fsm, container));
            
            Fsm.SetState<FsmStateWaveTimer>();
            
        }


        public void UpdateState()
        {
            Fsm?.Update();
        }
        public bool IsBegin()
        {
            return Fsm.StateCurrent.Value.GetType() == typeof(FsmStateWaveBegin);
        }
        public bool IsGo()
        {
            return Fsm.StateCurrent.Value.GetType() == typeof(FsmStateWaveGo);
        }
        public bool IsEnd()
        {
            return Fsm.StateCurrent.Value.GetType() == typeof(FsmStateWaveEnd);
        }
        public bool IsWait()
        {
            return Fsm.StateCurrent.Value.GetType() == typeof(FsmStateWaveWait);
        }
        
        public bool IsTimer()
        {
            return Fsm.StateCurrent.Value.GetType() == typeof(FsmStateWaveTimer);
        }
    }
}