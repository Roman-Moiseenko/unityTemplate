using UnityEngine;

namespace Game.GamePlay.FSM.Play
{
    public class FSMGameplay : MonoBehaviour
    {
        private MVVM.FSM.FSM _fsm;

        public void Bind()
        {
            _fsm = new MVVM.FSM.FSM();
            _fsm.AddState(new FSMGamePause(_fsm));
            _fsm.AddState(new FSMGameBuild(_fsm));
            _fsm.AddState(new FSMGameSpeedNormal(_fsm));
            _fsm.AddState(new FSMGameSpeed2x(_fsm));
            _fsm.AddState(new FSMGameSpeed4x(_fsm));
            _fsm.SetState<FSMGameBuild>();
        }

        private void Update()
        {
            _fsm.Update();
        }
    }
    
    
}