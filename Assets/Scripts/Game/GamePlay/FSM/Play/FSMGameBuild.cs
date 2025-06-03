using MVVM.FSM;
using UnityEngine;

namespace Game.GamePlay.FSM.Play
{
    public class FSMGameBuild : FSMState
    {
        public FSMGameBuild(MVVM.FSM.FSM fsm) : base(fsm)
        {

        }

        public override void Enter()
        {
            Debug.Log($"Начинаем строить");
            
        }

        public override void Exit()
        {
            Debug.Log($"Закончили строить");
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