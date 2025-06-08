using DI;

namespace MVVM.FSM
{
    public abstract class FSMState
    {
        public readonly FsmProxy Fsm;
        public object Params { get; set; }
        protected readonly DIContainer _container;

        public FSMState(FsmProxy fsm, DIContainer container)
        {
            Fsm = fsm;
            _container = container;
        }

        public virtual void Enter() { }

        public virtual bool Exit(FSMState _next = null)
        {
            if (_next == null) return true;
            return false;
        }
        public virtual void Update() { }
    }
}