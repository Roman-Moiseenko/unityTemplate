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
       //     Debug.Log("FsmStateBuildEnd Enter");
            //Fsm.SetState<FsmStateGamePlay>();
            
        }

        public override bool Exit(FSMState next)
        {
            if (next.GetType() == typeof(FsmStateGamePlay))
            {
          //      Debug.Log("FsmStateBuildEnd Exit");
                //_container.Resolve<IGameStateProvider>().GameplayState.GameplayReturn();
                return true;
            };
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