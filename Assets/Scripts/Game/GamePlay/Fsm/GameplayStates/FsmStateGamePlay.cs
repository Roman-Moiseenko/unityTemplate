using DI;
using Game.State;
using MVVM.FSM;
using UnityEngine;

namespace Game.GamePlay.Fsm.GameplayStates
{
    public class FsmStateGamePlay : FSMState
    {
        public FsmStateGamePlay(FsmProxy fsm, DIContainer container) : base(fsm, container)
        {
        }

        public override void Enter()
        {
            //Debug.Log(Fsm.PreviousState);
            if (Fsm.PreviousState != null)
            {
                //Debug.Log("GameplayReturn");
                _container.Resolve<IGameStateProvider>().GameplayState.GameplayReturn();
            }
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