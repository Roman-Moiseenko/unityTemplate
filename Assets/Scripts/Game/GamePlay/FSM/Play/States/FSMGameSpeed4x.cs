using MVVM.FSM;
using UnityEngine;

namespace Game.GamePlay.FSM.Play.States
{
    public class FSMGameSpeed4x : FSMState
    {
        public FSMGameSpeed4x(MVVM.FSM.FSM fsm) : base(fsm)
        {

        }

        public override void Enter()
        {
            Debug.Log($"Скорость установлена 4");
            Time.timeScale = 4;
        }

        public override void Exit()
        {
            Debug.Log($"Меняем скорость игры");
            //Запомнить скорость игры
        }

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Fsm.SetState<FSMGamePause>(this);
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Fsm.SetState<FSMGameSpeedNormal>();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Fsm.SetState<FSMGameSpeed2x>();
            }
        }
    }
}