using System;
using System.Collections.Generic;
using ObservableCollections;
using R3;
using UnityEngine;

namespace MVVM.FSM
{
    public class FsmProxy
    {
        //public FSM Origin;

        public ReactiveProperty<FSMState> StateCurrent = new();
        public FSMState PreviousState { get; private set; }
        private Dictionary<Type, FSMState> _states = new();

        public FsmProxy()
        {
            //Origin = fsm;
       //     StateCurrent = new ReactiveProperty<FSMState>();
       
            StateCurrent.Subscribe(newValue =>
            {
                if (newValue == null) return;
                Debug.Log($"Изменилось состояние {newValue.GetType()}");
            });
        }
        
        public void AddState(FSMState state)
        {
            _states.Add(state.GetType(), state);
        }
        
        public void SetState<T>() where T : FSMState
        {
            var type = typeof(T);
            if (StateCurrent.Value != null && StateCurrent.Value.GetType() == type) return;

            //Debug.Log(StateCurrent.Value.GetType());
            
//            if (StateCurrent?.Value == null) return;
            if (_states.TryGetValue(type, out var newState))
            {
                if (StateCurrent.Value == null) //Текущего состояния еще нет, сохраняем и входим в него
                {
                    StateCurrent.Value = newState;
                    StateCurrent.Value.Enter();
                    return;
                }
                
                //Проверка на выход из состояния, можно ли перейти к следующему состоянию
                if (StateCurrent.Value.Exit(newState)) //Если можно выйти из текущего состояния
                {
                    PreviousState = StateCurrent.Value;
                    //StateCurrent.Value?.Exit();
                    StateCurrent.Value = newState;
                    StateCurrent.Value.Enter();
                }
                
            }
            
        }

        public void Update()
        {
            StateCurrent.Value?.Update();
        }
        
    }
}