using R3;
using UnityEngine;

namespace Game.State.Root
{
    public class GameplayState
    {
        private readonly GameplayStateData _gameplayStateData;

        public readonly ReactiveProperty<int> GameSpeed;
        
        public GameplayState(GameplayStateData gameplayStateData)
        {
            _gameplayStateData = gameplayStateData;
            GameSpeed = new ReactiveProperty<int>(gameplayStateData.GameSpeed);
            GameSpeed.Subscribe(newSpeed =>
            {
                gameplayStateData.GameSpeed = newSpeed;
            });
        }

        /**
         * Ставим игру на паузу. Все объекты, которые зависят от скорости игры, подписываются на GameSpeed
         */
        public void SetPauseGame() 
        {
            _gameplayStateData.PreviousGameSpeed = GameSpeed.Value;
            GameSpeed.Value = 0;
        }
        
        /**
         * Возвращаемся к игре
         */
        public void GameplayReturn() 
        {
            if (_gameplayStateData.PreviousGameSpeed == null)
            {
                GameSpeed.Value = 1;
            } else
            {
                GameSpeed.Value = _gameplayStateData.PreviousGameSpeed;
            }
        }

        public void SetGameSpeed(int newSpeed)
        {
            if (newSpeed == GameSpeed.Value) return;
            GameSpeed.Value = newSpeed;
        }


        public int GetCurrentSpeed()
        {
            return GameSpeed.Value;
        }
    }
}