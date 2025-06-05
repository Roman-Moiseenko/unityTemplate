using System;
using DI;
using Game.Settings;
using Game.State;
using Game.State.Root;
using MVVM.FSM;
using UnityEngine;

namespace Game.GamePlay.Fsm.States
{
    public class FsmStateBuildBegin : FSMState
    {
        private int _previousGameSpeed;
        private GameplayState _gameplayState;

        public FsmStateBuildBegin(FsmProxy fsm, DIContainer container) : base(fsm, container)
        {
        }

        public override void Enter()
        {
            _gameplayState = _container.Resolve<IGameStateProvider>().GameState.GameplayState;
            _gameplayState.SetPauseGame();
        }

        public override bool Exit(FSMState _next)
        {
            if (_next.GetType() == typeof(FsmStateBuild)) return true;
            if (_next.GetType() == typeof(FsmStateBuildEnd)) return true;
            return false;
        }

        public override void Update()
        {
        }
    }
}