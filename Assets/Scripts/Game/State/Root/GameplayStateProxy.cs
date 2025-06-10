using R3;
using UnityEngine;

namespace Game.State.Root
{
    public class GameplayStateProxy
    {
        private readonly GameplayState _gameplayState;

        public readonly ReactiveProperty<int> GameSpeed;
        public readonly ReactiveProperty<int> Progress;
        public readonly ReactiveProperty<int> ProgressLevel;
        
        public GameplayStateProxy(GameplayState gameplayState)
        {
            _gameplayState = gameplayState;
            GameSpeed = new ReactiveProperty<int>(gameplayState.GameSpeed);
            GameSpeed.Subscribe(newSpeed => gameplayState.GameSpeed = newSpeed);
            
            Progress = new ReactiveProperty<int>(gameplayState.Progress);
            Progress.Subscribe(newProgress => gameplayState.Progress = newProgress);
            ProgressLevel = new ReactiveProperty<int>(gameplayState.ProgressLevel);
            ProgressLevel.Subscribe(newProgressLevel => gameplayState.ProgressLevel = newProgressLevel);
            
            
            
            InitMaps(gameplayState);
        }

        private void InitMaps(GameplayState gameplayState)
        {
            
            /*
            gameplayState.Maps.ForEach(
                mapOriginal => Maps.Add(new Map(mapOriginal))
            );
            Maps.ObserveAdd().Subscribe(e => gameState.Maps.Add(e.Value.Origin));

            Maps.ObserveRemove().Subscribe(e =>
            {
                var removedMapState = gameplayState.Maps.FirstOrDefault(b => b.Id == e.Value.Id);
                gameplayState.Maps.Remove(removedMapState);
            });
            */
        }


        public void ClearProgress()
        {
            if (Progress.Value < 100) return;
            Progress.Value -= 100;
            ProgressLevel.Value++;
        }
        
        /**
         * Ставим игру на паузу. Все объекты, которые зависят от скорости игры, подписываются на GameSpeed
         */
        public void SetPauseGame() 
        {
            _gameplayState.PreviousGameSpeed = GameSpeed.Value;
            GameSpeed.Value = 0;
        }
        
        /**
         * Возвращаемся к игре
         */
        public void GameplayReturn() 
        {
            if (_gameplayState.PreviousGameSpeed == 0)
            {
                GameSpeed.Value = 1;
            } else
            {
                GameSpeed.Value = _gameplayState.PreviousGameSpeed;
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

        public int SetNextSpeed()
        {
            var newSpeed = 1;
            switch (GameSpeed.Value)
            {
                case 1: newSpeed = 2;
                    break;
                case 2: newSpeed = 4;
                    break;
                case 4: newSpeed = 1;
                    break;
            }
          
            SetGameSpeed(newSpeed);
            return newSpeed;
        }
    }
}