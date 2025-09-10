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
        private GameplayStateProxy _gameplayStateProxy;

        public FsmStateSelectSkill(FsmProxy fsm, DIContainer container) : base(fsm, container)
        {
        }

        public override void Enter()
        {
            _gameplayStateProxy = _container.Resolve<IGameStateProvider>().GameplayState; 
            //_previousGameSpeed = _gameplayStateProxy.GetCurrentSpeed(); //Запоминаем текущую скорость
            _gameplayStateProxy.SetSkillSpeed(); //Устанавливаем минимальную скорость
        }

        public override bool Exit(FSMState next = null)
        {
            if (next == null) return false;
            _gameplayStateProxy.GameplayReturn(); //Возвращаем скорость
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