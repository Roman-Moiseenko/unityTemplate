using System;
using DI;
using Game.Settings;
using Game.State;
using Game.State.Root;
using MVVM.FSM;

namespace Game.GamePlay.Fsm.States
{
    public class FsmStateSelectSkill : FSMState
    {
        private int _previousGameSpeed;
        private GameplayState _gameplayState;

        public FsmStateSelectSkill(FsmProxy fsm, DIContainer container) : base(fsm, container)
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
            //Любой режим может сменить текущее состояние, и скилл не применяется, если _next != FsmStateSetSkill
            /*
            if (_next.GetType() == typeof(FsmStateSetSkill))
            {
                //Применяем скилл
                _gameplayState.SetGameSpeed(_previousGameSpeed); //Возвращаем скорость
                return true;
            }
            if (_next.GetType() == typeof(FsmStateGamePlay))
            {
                //Возвращаемся в игру, без применения скила
                _gameplayState.SetGameSpeed(_previousGameSpeed); //Возвращаем скорость
                return true;
            }
            if (_next.GetType() == typeof(FsmStateBuildBegin))
            {
                //Переходим в режим строительства, без применения скила
                _gameplayState.SetGameSpeed(_previousGameSpeed); //Возвращаем скорость
                return true;
            }
            
            return false;*/
        }

        public override void Update()
        {
        }
    }
}