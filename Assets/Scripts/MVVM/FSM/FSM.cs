using System;
using System.Collections.Generic;

namespace MVVM.FSM
{
    public class FSM
    {
        //TODO Нужна подписка
        public FSMState StateCurrent { get; private set; }
        public FSMState PreviousState { get; private set; }
        public Dictionary<Type, FSMState> States = new();

        public void AddState(FSMState state)
        {
            States.Add(state.GetType(), state);
        }
        
        public void SetState<T>() where T : FSMState
        {
            var type = typeof(T);
            if (StateCurrent != null && StateCurrent.GetType() == type) return;

            
            if (States.TryGetValue(type, out var newState))
            {
                PreviousState = StateCurrent;
                
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