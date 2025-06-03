namespace MVVM.FSM
{
    public abstract class FSMState
    {
        public readonly FSM Fsm;

        public FSMState(FSM fsm)
        {
            Fsm = fsm;
        }

        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void Update() { }
    }
}