using DI;
using Game.GamePlay.Fsm.HeroStates;
using Game.GamePlay.Fsm.SkillStates;
using Game.GamePlay.Fsm.TowerStates;
using Game.State;
using Game.State.Gameplay;
using Game.State.Root;
using MVVM.FSM;
using UnityEngine;

namespace Game.GamePlay.Fsm.GameplayStates
{
    public class FsmStateBuildBegin : FSMState
    {
        private int _previousGameSpeed;
        private GameplayStateProxy _gameplayStateProxy;
        

        public FsmStateBuildBegin(FsmProxy fsm, DIContainer container) : base(fsm, container)
        {

        }

        public override void Enter()
        {
          //  Debug.Log("FsmStateBuildBegin " + _container.Resolve<IGameStateProvider>().GameplayState.GameSpeed.Value);
            _container.Resolve<IGameStateProvider>().GameplayState.SetPauseGame();
            //При начале строительства сбрасываем выделения
            var fsmHero = _container.Resolve<FsmHero>();
            var fsmTower = _container.Resolve<FsmTower>();
            var fsmSkill = _container.Resolve<FsmSkill>();
            if (fsmHero.IsSelected()) fsmHero.Fsm.SetState<FsmHeroUnSelected>();
            if (fsmTower.IsSelected()) fsmTower.Fsm.SetState<FsmTowerNone>();
            if(fsmSkill.IsBegin()) fsmSkill.Fsm.SetState<FsmSkillNone>();
            
        }

        public override bool Exit(FSMState next = null)
        {
            if (next == null) return false;
            return next.GetType() == typeof(FsmStateBuild) || next.GetType() == typeof(FsmStateBuildEnd);
        }

        public override void Update() { }
    }
}