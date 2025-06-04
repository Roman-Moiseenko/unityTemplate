using MVVM.FSM;
using UnityEngine;

namespace Game.GamePlay.FSM.Play.States
{
    public class FSMGamePause : FSMState
    {
        
        public FSMGamePause(MVVM.FSM.FSM fsm) : base(fsm) { }

        public override void Enter()
        {
            Debug.Log($"Игра на паузе");
        }
        public override void Exit()
        {
            Debug.Log($"Игра возобновлена");
        }
        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (Fsm.PreviousState.GetType() == typeof(FSMGameSpeedNormal))
                {
                    Fsm.SetState<FSMGameSpeedNormal>();
                    return;
                }
                if (Fsm.PreviousState.GetType() == typeof(FSMGameSpeed2x))
                {
                    Fsm.SetState<FSMGameSpeed2x>();
                    return;
                }
                if (Fsm.PreviousState.GetType() == typeof(FSMGameSpeed4x))
                {
                    Fsm.SetState<FSMGameSpeed4x>();
                    return;
                }
                
            }
        }
    }
}