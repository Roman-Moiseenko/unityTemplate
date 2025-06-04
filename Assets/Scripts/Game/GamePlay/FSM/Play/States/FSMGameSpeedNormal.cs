using MVVM.FSM;
using UnityEngine;

namespace Game.GamePlay.FSM.Play.States
{
    public class FSMGameSpeedNormal : FSMState
    {
        public FSMGameSpeedNormal(MVVM.FSM.FSM fsm) : base(fsm)
        {

        }

        public override void Enter()
        {
            Debug.Log($"Скорость установлена 1");
            Time.timeScale = 1;
        }

        public override void Exit()
        {
            Debug.Log($"Меняем скорость игры");
        }

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Fsm.SetState<FSMGamePause>(this);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Fsm.SetState<FSMGameSpeed2x>();
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Fsm.SetState<FSMGameSpeed4x>();
            }
        }
    }
}