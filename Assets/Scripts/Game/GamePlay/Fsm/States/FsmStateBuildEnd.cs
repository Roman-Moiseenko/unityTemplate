using System;
using DI;
using Game.Settings;
using Game.State;
using Game.State.Gameplay;
using Game.State.Root;
using MVVM.FSM;

namespace Game.GamePlay.Fsm.States
{
    public class FsmStateBuildEnd : FSMState
    {
        private int _previousGameSpeed;
        private GameplayStateProxy _gameplayStateProxy;

        public FsmStateBuildEnd(FsmProxy fsm, DIContainer container) : base(fsm, container)
        {
        }

        public override void Enter()
        {
            //Входим в состояние Завершения строительства
            //Все подписчики его обрабатывают.
            //Переходим в состояние Игры
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
        
        public RewardCardData GetRewardCard()
        {
            return (RewardCardData)Params;
        }
    }
}