using DI;
using Game.State.Root;
using MVVM.FSM;

namespace Game.GamePlay.Fsm.GameplayStates
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

        public override bool Exit(FSMState next = null)
        {
            if (next == null) return false;
            return next.GetType() == typeof(FsmStateGamePlay);
        }

        public override void Update()
        {
        }
    }
}