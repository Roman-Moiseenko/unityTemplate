using DI;

namespace MVVM.FSM
{
    public abstract class FSMState
    {
        public readonly FsmProxy Fsm;
        public object Params { get; set; }
        protected readonly DIContainer _container;

        protected FSMState(FsmProxy fsm, DIContainer container)
        {
            Fsm = fsm;
            _container = container;
        }

        public virtual void Enter() { }

        public virtual bool Exit(FSMState next = null)
        {
            return next == null;
        }
        public virtual void Update() { }
    }
}