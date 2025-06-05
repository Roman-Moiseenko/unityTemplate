using System;
using DI;
using Game.Settings;
using Game.State;
using Game.State.Root;
using MVVM.FSM;

namespace Game.GamePlay.Fsm.States
{
    public class FsmStateSkill : FSMState
    {
        private int _previousGameSpeed;
        private GameplayState _gameplayState;

        public FsmStateSkill(FsmProxy fsm, DIContainer container) : base(fsm, container)
        {
        }

        public override void Enter()
        {
            _gameplayState = _container.Resolve<IGameStateProvider>().GameState.GameplayState; 
            _previousGameSpeed = _gameplayState.GetCurrentSpeed(); //Запоминаем текущую скорость
            _gameplayState.SetGameSpeed(1); //Устанавливаем минимальную скорость
        }

        public override bool Exit(FSMState _next)
        {
            
            _gameplayState.SetGameSpeed(_previousGameSpeed); //Возвращаем скорость
            return true;
        }

        public override void Update()
        {
        }
    }
}