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
          //  Debug.Log("FsmStateGamePlay Enter");
            if (Fsm.PreviousState != null)
            {
          //      Debug.Log("FsmStateGamePlay Enter");
                _container.Resolve<IGameStateProvider>().GameplayState.GameplayReturn();
            }
            
        }

        public override bool Exit(FSMState _next)
        {
          //  Debug.Log("FsmStateGamePlay Exit");
            if (_next.GetType() == typeof(FsmStateGamePause)) return true;
            if (_next.GetType() == typeof(FsmStateSelectSkill)) return true;
            if (_next.GetType() == typeof(FsmStateBuildBegin)) return true;
            
            return false;
        }

        public override void Update()
        {
            
        }
    }
}