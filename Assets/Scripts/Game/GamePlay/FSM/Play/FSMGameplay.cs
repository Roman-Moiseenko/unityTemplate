using DI;
using Game.GamePlay.FSM.Play.States;
using MVVM.FSM;
using UnityEngine.PlayerLoop;

namespace Game.GamePlay.FSM.Play
{
    public class FSMGameplay
    {
        public MVVM.FSM.FSM _fsm { get; }

        public FSMGameplay(DIContainer container)
        {
            _fsm = new MVVM.FSM.FSM();
            _fsm.AddState(new FSMGamePause(_fsm));
            _fsm.AddState(new FSMGameReturn(_fsm));
            _fsm.AddState(new FSMGameBuild(_fsm));
            _fsm.AddState(new FSMGameSpeedNormal(_fsm));
            _fsm.AddState(new FSMGameSpeed2x(_fsm));
            _fsm.AddState(new FSMGameSpeed4x(_fsm));
            _fsm.SetState<FSMGameBuild>();
        }

        public FSMState GetCurrentState()
        {
            return _fsm.StateCurrent;
        }
        
        public FSMState GetPreviousState()
        {
            return _fsm.PreviousState;
        }

        public void UpdateState()
        {
            _fsm?.Update();
        }
        
        
    }
}