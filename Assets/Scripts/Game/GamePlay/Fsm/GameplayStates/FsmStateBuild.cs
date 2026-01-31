using DI;
using Game.GamePlay.Fsm.TowerStates;
using Game.State.Gameplay;
using Game.State.Root;
using MVVM.FSM;

namespace Game.GamePlay.Fsm.GameplayStates
{
    public class FsmStateBuild : FSMState
    {
        private int _previousGameSpeed;
        private GameplayStateProxy _gameplayStateProxy;

        public FsmStateBuild(FsmProxy fsm, DIContainer container) : base(fsm, container) { }

        public override void Enter()
        {
            //При строительстве закрываем состояние башни
            var fsmTower = _container.Resolve<FsmTower>();
            if (!fsmTower.IsNone()) fsmTower.Fsm.SetState<FsmTowerNone>();
        }

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