using System;
using System.Collections.Generic;
using Newtonsoft.Json;
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
        public ReactiveProperty<Vector2Int> Position = new();
        public object Params { get; private set; }
        private Dictionary<Type, FSMState> _states = new();

        public FsmProxy()
        {
            StateCurrent.Subscribe(newValue =>
            {
                if (newValue == null) return;
            });
        }

        public void AddState(FSMState state)
        {
            _states.Add(state.GetType(), state);
        }

        public void SetState<T>(object enterParams = null) where T : FSMState
        {
            //  Debug.Log("SetState = " + typeof(T));
            Params = enterParams;
            //if (enterParams != null) Debug.Log(JsonConvert.SerializeObject(enterParams, Formatting.Indented));

            var type = typeof(T);
            if (StateCurrent.Value != null && StateCurrent.Value.GetType() == type) return;

            //Debug.Log(StateCurrent.Value.GetType());

//            if (StateCurrent?.Value == null) return;
            if (_states.TryGetValue(type, out var newState))
            {
                if (StateCurrent.Value == null) //Текущего состояния еще нет, сохраняем и входим в него
                {
                }

                //Проверка на выход из состояния, можно ли перейти к следующему состоянию
                if (StateCurrent.Value != null) //Текущее состояние уже есть, то проверяем, 
                {
                    //     Debug.Log("Проверка на выход и тек.состояния");
                    if (!StateCurrent.Value.Exit(newState))
                    {
                        //    Debug.Log("Выйти нельзя, возврат к текущему");
                        return;
                    } //- можно ли из него выйти

                    PreviousState = StateCurrent.Value; //- сохраняем его (если понадобится)
            //        Debug.Log("Вышли");
                }

                if (enterParams != null)
                    newState.Params = enterParams; //Чтоб не перезаписать, при возврате к BuildBegin

                newState.Enter(); //Сначала входим в новое состояние
                StateCurrent.Value = newState; //Затем запоминаем, для подписок

                //   Debug.Log(newState.GetType());
                //StateCurrent.Value.Params = enterParams;
                // Debug.Log("SetState = " + typeof(T) + "\n перед Enter");
                //StateCurrent.Value.Enter();
            }
        }

        public void Update()
        {
            StateCurrent.Value?.Update();
        }
    }
}