using MVVM.FSM;
using UnityEngine;

namespace Game.GamePlay.FSM.Play
{
    public class FSMGameSpeed2x : FSMState
    {
        public FSMGameSpeed2x(MVVM.FSM.FSM fsm) : base(fsm)
        {

        }

        public override void Enter()
        {
            Debug.Log($"Скорость установлена 2");
            Time.timeScale = 2;
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
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Fsm.SetState<FSMGameSpeed4x>();
            }
        }
    }
}