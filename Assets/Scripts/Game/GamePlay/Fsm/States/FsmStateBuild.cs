using System;
using DI;
using Game.Settings;
using Game.State;
using Game.State.Gameplay;
using Game.State.Root;
using MVVM.FSM;

namespace Game.GamePlay.Fsm.States
{
    public class FsmStateBuild : FSMState
    {
        private int _previousGameSpeed;
        private GameplayStateProxy _gameplayStateProxy;

        public FsmStateBuild(FsmProxy fsm, DIContainer container) : base(fsm, container) { }

        public override void Enter() { }

        public override bool Exit(FSMState next = null)
        {
            if (next == null) return false;
            return next.GetType() == typeof(FsmStateBuildBegin) || next.GetType() == typeof(FsmStateBuildEnd);
        }

        public override void Update() { }

        public RewardCardData GetRewardCard()
        {
            return (RewardCardData)Params;
        }
    }
}