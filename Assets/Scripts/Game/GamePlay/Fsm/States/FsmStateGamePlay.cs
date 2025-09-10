using DI;
using Game.State;
using MVVM.FSM;
using UnityEngine;

namespace Game.GamePlay.Fsm.States
{
    public class FsmStateGamePlay : FSMState
    {
        public FsmStateGamePlay(FsmProxy fsm, DIContainer container) : base(fsm, container)
        {
        }

        public override void Enter()
        {
            if (Fsm.PreviousState != null)
                _container.Resolve<IGameStateProvider>().GameplayState.GameplayReturn();
        }

        public override bool Exit(FSMState next = null)
        {
            if (next == null) return false;

            return next.GetType() == typeof(FsmStateGamePause) ||
                   next.GetType() == typeof(FsmStateSelectSkill) ||
                   next.GetType() == typeof(FsmStateBuildBegin);
        }

        public override void Update()
        {
        }
    }
}