using System;
using System.Collections.Generic;

namespace MVVM.FSM
{
    public class FSM
    {
        public FSMState StateCurrent { get; private set; }
        public FSMState PreviousState { get; private set; }
        private Dictionary<Type, FSMState> _states = new();

        public void AddState(FSMState state)
        {
            _states.Add(state.GetType(), state);
        }
        
        public void SetState<T>(FSMState previousState = null) where T : FSMState
        {
            var type = typeof(T);
            if (StateCurrent != null && StateCurrent.GetType() == type) return;

            if (_states.TryGetValue(type, out var newState))
            {
                PreviousState = previousState; //Запоминаем предыдущее состояние
                StateCurrent?.Exit();
                StateCurrent = newState;
                StateCurrent.Enter();
            }
            
        }

        public void Update()
        {
            StateCurrent?.Update();
        }

    }
}