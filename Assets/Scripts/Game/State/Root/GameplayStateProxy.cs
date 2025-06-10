using System.Linq;
using Game.State.Entities;
using ObservableCollections;
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
        public readonly ReactiveProperty<int> SoftCurrency;
        public readonly ReactiveProperty<int> MapId;
        public readonly ReactiveProperty<int> CurrentWave;

        public int PreviousGameSpeed => _gameplayState.PreviousGameSpeed;
        
        public ObservableList<Entity> Entities { get; } = new();
        
        public GameplayStateProxy(GameplayState gameplayState)
        {
            _gameplayState = gameplayState;
            GameSpeed = new ReactiveProperty<int>(gameplayState.GameSpeed);
            GameSpeed.Subscribe(newSpeed =>
            {
                gameplayState.GameSpeed = newSpeed;
                //Debug.Log($"Новая гейплей скорость = {newSpeed}");
            });
            
            Progress = new ReactiveProperty<int>(gameplayState.Progress);
            Progress.Subscribe(newProgress => gameplayState.Progress = newProgress);
            ProgressLevel = new ReactiveProperty<int>(gameplayState.ProgressLevel);
            ProgressLevel.Subscribe(newProgressLevel => gameplayState.ProgressLevel = newProgressLevel);

            SoftCurrency = new ReactiveProperty<int>(gameplayState.SoftCurrency);
            SoftCurrency.Subscribe(newValue => gameplayState.SoftCurrency = newValue);

            CurrentWave = new ReactiveProperty<int>(gameplayState.CurrentWave);
            CurrentWave.Subscribe(newValue => gameplayState.CurrentWave = newValue);
            

             InitMaps(gameplayState);
        }

        private void InitMaps(GameplayState gameplayState)
        {
            gameplayState.Entities.ForEach(
                entityOriginal => Entities.Add(EntitiesFactory.CreateEntity(entityOriginal))
            );
            Entities.ObserveAdd().Subscribe(e => gameplayState.Entities.Add(e.Value.Origin));

            Entities.ObserveRemove().Subscribe(e =>
            {
                var removedMapState = gameplayState.Entities.FirstOrDefault(b => b.UniqueId == e.Value.UniqueId);
                gameplayState.Entities.Remove(removedMapState);
            });
            
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
        
        public int CreateEntityID()
        {
            return _gameplayState.CreateEntityID();
        }
    }
}