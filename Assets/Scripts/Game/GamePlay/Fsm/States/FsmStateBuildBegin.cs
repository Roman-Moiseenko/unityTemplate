using System;
using DI;
using Game.Settings;
using Game.State;
using Game.State.Gameplay;
using Game.State.Root;
using MVVM.FSM;
using UnityEngine;

namespace Game.GamePlay.Fsm.States
{
    public class FsmStateBuildBegin : FSMState
    {
        private int _previousGameSpeed;
        private GameplayStateProxy _gameplayStateProxy;
      //  public override RewardsProgress Params { get; set; }

        public FsmStateBuildBegin(FsmProxy fsm, DIContainer container) : base(fsm, container) { }

        public override void Enter()
        {
            _gameplayStateProxy = _container.Resolve<IGameStateProvider>().GameplayState;
            _gameplayStateProxy.SetPauseGame();
        }

        public override bool Exit(FSMState _next)
        {
            if (_next.GetType() == typeof(FsmStateBuild)) return true;
            if (_next.GetType() == typeof(FsmStateBuildEnd)) return true;
            return false;
        }

        public override void Update() { }

        public RewardsProgress GetRewards()
        {
            return (RewardsProgress)Params;
        }
    }
}