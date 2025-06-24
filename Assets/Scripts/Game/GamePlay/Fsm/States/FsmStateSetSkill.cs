using System;
using DI;
using Game.Settings;
using Game.State;
using Game.State.Root;
using MVVM.FSM;

namespace Game.GamePlay.Fsm.States
{
    public class FsmStateSetSkill : FSMState
    {
        private int _previousGameSpeed;
        private GameplayStateProxy _gameplayStateProxy;

        public FsmStateSetSkill(FsmProxy fsm, DIContainer container) : base(fsm, container)
        {
        }

        public override void Enter()
        {
            //При входе, по подписке Скилл применяется в своем контроллере,
            //Текущее состояние сразу меняем на GamePlay
            Fsm.SetState<FsmStateGamePlay>();
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