using DI;
using Game.State.Gameplay;
using Game.State.Root;
using MVVM.FSM;

namespace Game.GamePlay.Fsm.GameplayStates
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

        public override bool Exit(FSMState next = null)
        {
            if (next == null) return false;
            return next.GetType() == typeof(FsmStateGamePlay);
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