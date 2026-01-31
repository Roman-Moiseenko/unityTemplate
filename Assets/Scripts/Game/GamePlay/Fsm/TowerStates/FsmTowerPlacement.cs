using DI;
using Game.State;
using MVVM.FSM;

namespace Game.GamePlay.Fsm.TowerStates
{
    public class FsmTowerPlacement : FSMState
    {
        public FsmTowerPlacement(FsmProxy fsm, DIContainer container) : base(fsm, container)
        {
            
        }

        public override void Enter()
        {
            //При размещении точки возрождения ставим на паузу
            _container.Resolve<IGameStateProvider>().GameplayState.SetPauseGame();
        }

        public override bool Exit(FSMState next = null)
        {
            var fsmGameplay = _container.Resolve<FsmGameplay>();
            //При выходе из размещения точки возрождения снимаем с паузы, если был режим игры
            if (fsmGameplay.IsStateGaming())
            {
                _container.Resolve<IGameStateProvider>().GameplayState.GameplayReturn();    
            }
            
            return true;
        }

        public override void Update() { }
    }
}