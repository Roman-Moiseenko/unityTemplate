using System.Linq;
using Game.State.Entities;
using Game.State.Maps.Castle;
using Game.State.Maps.Grounds;
using Game.State.Maps.Roads;
using Newtonsoft.Json;
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

        public readonly ReactiveProperty<CastleEntity> Castle;

        public int PreviousGameSpeed => _gameplayState.PreviousGameSpeed;
        
        public ObservableList<Entity> Entities { get; } = new();
        public ObservableList<GroundEntity> Grounds { get; } = new();

        public ObservableList<RoadEntity> Way { get; } = new();
        public ObservableList<RoadEntity> WaySecond { get; } = new();
        public ObservableList<RoadEntity> WayDisabled { get; } = new();
        
        public GameplayStateProxy(GameplayState gameplayState)
        {
            _gameplayState = gameplayState;
            Castle = new ReactiveProperty<CastleEntity>(new CastleEntity(gameplayState.CastleData));
            
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

         //   Debug.Log("gameplayState = " + JsonConvert.SerializeObject(gameplayState, Formatting.Indented));
            InitMaps(gameplayState);
        }

        private void InitMaps(GameplayState gameplayState)
        {
            gameplayState.Grounds.ForEach(
                groundOriginal => Grounds.Add(new GroundEntity(groundOriginal))
            );
            Grounds.ObserveAdd().Subscribe(e => gameplayState.Grounds.Add(e.Value.Origin));

            Grounds.ObserveRemove().Subscribe(e =>
            {
                var removedMapState = gameplayState.Grounds.FirstOrDefault(b => b.UniqueId == e.Value.UniqueId);
                gameplayState.Grounds.Remove(removedMapState);
            });
            
            
            gameplayState.Entities.ForEach(
                entityOriginal => Entities.Add(EntitiesFactory.CreateEntity(entityOriginal))
            );
            Entities.ObserveAdd().Subscribe(e => gameplayState.Entities.Add(e.Value.Origin));

            Entities.ObserveRemove().Subscribe(e =>
            {
                var removedMapState = gameplayState.Entities.FirstOrDefault(b => b.UniqueId == e.Value.UniqueId);
                gameplayState.Entities.Remove(removedMapState);
            });
            
            
            //Дороги
            gameplayState.Way.ForEach(roadOriginal => Way.Add(new RoadEntity(roadOriginal)));
            Way.ObserveAdd().Subscribe(r => gameplayState.Way.Add(r.Value.Origin));
            Way.ObserveRemove().Subscribe(r =>
            {
                var removedRoad = gameplayState.Way.FirstOrDefault(b => b.UniqueId == r.Value.UniqueId);
                gameplayState.Way.Remove(removedRoad);
            });
            
            gameplayState.WaySecond.ForEach(roadOriginal => WaySecond.Add(new RoadEntity(roadOriginal)));
            WaySecond.ObserveAdd().Subscribe(r => gameplayState.WaySecond.Add(r.Value.Origin));
            WaySecond.ObserveRemove().Subscribe(r =>
            {
                var removedRoad = gameplayState.WaySecond.FirstOrDefault(b => b.UniqueId == r.Value.UniqueId);
                gameplayState.WaySecond.Remove(removedRoad);
            });
            
            gameplayState.WayDisabled.ForEach(roadOriginal => WayDisabled.Add(new RoadEntity(roadOriginal)));
            WayDisabled.ObserveAdd().Subscribe(r => gameplayState.WayDisabled.Add(r.Value.Origin));
            WayDisabled.ObserveRemove().Subscribe(r =>
            {
                var removedRoad = gameplayState.WayDisabled.FirstOrDefault(b => b.UniqueId == r.Value.UniqueId);
                gameplayState.WayDisabled.Remove(removedRoad);
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