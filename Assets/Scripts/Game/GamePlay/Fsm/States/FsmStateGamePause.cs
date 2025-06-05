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
            _container.Resolve<IGameStateProvider>().GameState.GameplayState.SetPauseGame();
        }

        public override bool Exit(FSMState _next)
        {
            if (_next.GetType() == typeof(FsmStateGamePlay)) return true;
            return false;
        }

        public override void Update()
        {
            
        }
    }
}