using System.Collections;
using DI;
using Game.GamePlay.Fsm.States;
using MVVM.FSM;
using R3;
using UnityEngine;

namespace Game.GamePlay.Fsm
{
    public class FsmGameplay
    {
        public FsmProxy Fsm;

        public ReactiveProperty<bool> IsGamePause; //Во все состояния, кроме FsmStateGamePlay пауза для движения

        public FsmGameplay(DIContainer container)
        {
            Fsm = new FsmProxy();
            
            Fsm.AddState(new FsmStateGamePause(Fsm, container));
            Fsm.AddState(new FsmStateGamePlay(Fsm, container));
            Fsm.AddState(new FsmStateSelectSkill(Fsm, container));
            Fsm.AddState(new FsmStateSetSkill(Fsm, container));
            Fsm.AddState(new FsmStateBuildBegin(Fsm, container));
            Fsm.AddState(new FsmStateBuild(Fsm, container));
            Fsm.AddState(new FsmStateBuildEnd(Fsm, container));
            
            Fsm.SetState<FsmStateGamePlay>();

            IsGamePause = new ReactiveProperty<bool>();
            //Общедоступное реактивное свойство Игра на Паузе 
            Fsm.StateCurrent.Subscribe(newState =>
            {
                IsGamePause.Value = newState.GetType() != typeof(FsmStateGamePlay);
            });
        }

        public IEnumerator WaitPause()
        {
            while (IsGamePause.Value)//Пауза
            {
                yield return null;
            }
        }

        public void UpdateState()
        {
            Fsm?.Update();
        }      
        public bool IsPause()
        {
            return IsGamePause.CurrentValue;
        }

        public bool IsStateGamePlay()
        {
            return Fsm.StateCurrent.Value.GetType() == typeof(FsmStateGamePlay);
        }
        public bool IsStateGamePause()
        {
            return Fsm.StateCurrent.Value.GetType() == typeof(FsmStateGamePause);
        }
        
        public bool IsStateGaming()
        {
            return IsStateGamePlay() || IsStateGamePause();
        }

        public bool IsStateBuildBegin()
        {
            return Fsm.StateCurrent.Value.GetType() == typeof(FsmStateBuildBegin);
        }
        
        public bool IsStateBuild()
        {
            return Fsm.StateCurrent.Value.GetType() == typeof(FsmStateBuild);
        }
        public bool IsStateBuildEnd()
        {
            return Fsm.StateCurrent.Value.GetType() == typeof(FsmStateBuildEnd);
        }
        
        public bool IsStateBuilding()
        {
            return IsStateBuildBegin() || IsStateBuild() || IsStateBuildEnd();
        }

        public void SetPosition(Vector2Int position)
        {
            Fsm.Position.Value = position;
        }

        public Vector2Int GetPosition()
        {
            return Fsm.Position.CurrentValue;
        }
    }
}