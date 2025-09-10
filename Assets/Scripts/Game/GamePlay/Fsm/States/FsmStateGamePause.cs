using System;
using DI;
using Game.Settings;
using Game.State;
using MVVM.FSM;
using UnityEngine;

namespace Game.GamePlay.Fsm.States
{
    public class FsmStateGamePause : FSMState
    {
        public FsmStateGamePause(FsmProxy fsm, DIContainer container) : base(fsm, container)
        {
            
        }

        public override void Enter()
        {
            //Ставим игру на паузу
            _container.Resolve<IGameStateProvider>().GameplayState.SetPauseGame();
        }

        public override bool Exit(FSMState next = null)
        {
            if (next == null) return false;
            if (next.GetType() != typeof(FsmStateGamePlay)) return false;
            
            _container.Resolve<IGameStateProvider>().GameplayState.GameplayReturn();
            return true;
        }

        public override void Update()
        {
            
        }
    }
}