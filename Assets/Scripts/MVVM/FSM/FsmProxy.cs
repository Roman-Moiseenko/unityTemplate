using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

namespace MVVM.FSM
{
    public class FsmProxy : IDisposable
    {
        //public FSM Origin;

        public ReactiveProperty<FSMState> StateCurrent = new();
        public FSMState PreviousState { get; private set; }
        public ReactiveProperty<Vector2Int> Position = new();
        public object Params { get; private set; } //Хранение параметров для всех состояний
        private Dictionary<Type, FSMState> _states = new();

        public FsmProxy()
        {
            /*
            StateCurrent.Subscribe(newValue =>
            {
                if (newValue == null) return;
            });
            */
        }

        public object GetParamsState()
        {
            return StateCurrent.CurrentValue?.Params;
        }

        public void SetParamsState(object obj)
        {
            StateCurrent.Value.Params = obj;
        }

        public void AddState(FSMState state)
        {
            _states.Add(state.GetType(), state);
        }

        public void SetState<T>(object enterParams = null) where T : FSMState
        {
            if (enterParams != null) Params = enterParams;

            var type = typeof(T);
            if (StateCurrent.Value != null && StateCurrent.Value.GetType() == type) return;

            if (_states.TryGetValue(type, out var newState))
            {
                if (StateCurrent.Value == null) //Текущего состояния еще нет, сохраняем и входим в него
                {
                }

                //Проверка на выход из состояния, можно ли перейти к следующему состоянию
                if (StateCurrent.Value != null) //Текущее состояние уже есть, то проверяем, 
                {
                    if (!StateCurrent.Value.Exit(newState)) return; //- можно ли из него выйти
                    if (StateCurrent.Value != newState) //Если новое состояние отличается от текущего, то сохраняем его
                        PreviousState = StateCurrent.Value;
                }

                if (enterParams != null)
                    newState.Params = enterParams; //Чтоб не перезаписать, при возврате к BuildBegin

                newState.Enter(); //Сначала входим в новое состояние
                StateCurrent.OnNext(newState); //Затем запоминаем, для подписок, принудительно
            }
        }

        public void Update()
        {
            StateCurrent.Value?.Update();
        }

/*
        public void ClearParams()
        {
            Params = null;
        }*/
        public void Dispose()
        {
            StateCurrent?.Dispose();
            Position?.Dispose();
        }
    }
}