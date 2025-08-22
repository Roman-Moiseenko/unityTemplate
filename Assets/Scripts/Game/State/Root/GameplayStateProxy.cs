using System;
using System.Linq;
using Game.State.Entities;
using Game.State.Maps.Castle;
using Game.State.Maps.Grounds;
using Game.State.Maps.Roads;
using Game.State.Maps.Towers;
using Game.State.Maps.Waves;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.State.Root
{
    public class GameplayStateProxy
    {
        public readonly GameplayState Origin;

        public readonly ReactiveProperty<int> GameSpeed;
        public readonly ReactiveProperty<int> Progress;
        public readonly ReactiveProperty<int> ProgressLevel;
        public readonly ReactiveProperty<int> SoftCurrency;
        public readonly ReactiveProperty<int> MapId;
        public readonly ReactiveProperty<int> CurrentWave;
        public readonly ReactiveProperty<int> UpdateCards;

        public CastleEntity Castle;

        public int PreviousGameSpeed => Origin.PreviousGameSpeed;
        
        public ObservableList<TowerEntity> Towers { get; } = new();
        public ObservableList<GroundEntity> Grounds { get; } = new();

        public ObservableList<RoadEntity> Way { get; } = new();
        public ObservableList<RoadEntity> WaySecond { get; } = new();
        public ObservableList<RoadEntity> WayDisabled { get; } = new();
        public ObservableDictionary<int, WaveEntity> Waves { get; } = new();
        
        public GameplayStateProxy(GameplayState origin)
        {
            Origin = origin;
            Castle = new CastleEntity(origin.CastleData);
            
            GameSpeed = new ReactiveProperty<int>(origin.GameSpeed);
            GameSpeed.Subscribe(newSpeed =>
            {
                origin.GameSpeed = newSpeed;
//                Debug.Log($"Новая гейплей скорость = {newSpeed}");
            });
            
            Progress = new ReactiveProperty<int>(origin.Progress);
            Progress.Subscribe(newProgress => origin.Progress = newProgress);
            ProgressLevel = new ReactiveProperty<int>(origin.ProgressLevel);
            ProgressLevel.Subscribe(newProgressLevel => origin.ProgressLevel = newProgressLevel);

            SoftCurrency = new ReactiveProperty<int>(origin.SoftCurrency);
            SoftCurrency.Subscribe(newValue => origin.SoftCurrency = newValue);

            CurrentWave = new ReactiveProperty<int>(origin.CurrentWave);
            CurrentWave.Subscribe(newValue => origin.CurrentWave = newValue);

            UpdateCards = new ReactiveProperty<int>(origin.UpdateCards);
            UpdateCards.Subscribe(newValue => origin.UpdateCards = newValue);

            MapId = new ReactiveProperty<int>(origin.MapId);
            MapId.Subscribe(newValue => origin.MapId = newValue);
            
         //   Debug.Log("gameplayState = " + JsonConvert.SerializeObject(gameplayState, Formatting.Indented));
            InitMaps(origin);
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
            
            
            gameplayState.Towers.ForEach(
                towerOriginal => Towers.Add(new TowerEntity(towerOriginal))
            );
            Towers.ObserveAdd().Subscribe(e => gameplayState.Towers.Add(e.Value.Origin));

            Towers.ObserveRemove().Subscribe(e =>
            {
                var removedMapState = gameplayState.Towers.FirstOrDefault(b => b.UniqueId == e.Value.UniqueId);
                gameplayState.Towers.Remove(removedMapState);
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

            foreach (var stateWave in gameplayState.Waves)
            {
                Waves.Add(stateWave.Key, new WaveEntity(stateWave.Value));
            }
            //gameplayState.Waves.ForEach(wave => Waves.Add(new WaveEntity(wave)));
            Waves.ObserveAdd().Subscribe(r => gameplayState.Waves.Add(r.Value.Key, r.Value.Value.Origin));
            Waves.ObserveRemove().Subscribe(r =>
            {
                if (Waves.TryGetValue(r.Value.Key, out var waveEntity))
                {
                    gameplayState.Waves.Remove(r.Value.Key);
                }
                //var removedWave = gameplayState.Waves.FirstOrDefault(b => b.Number == r.Value.Number);
                //gameplayState.Waves.Remove(removedWave);
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
            Origin.PreviousGameSpeed = GameSpeed.Value;
            GameSpeed.Value = 0;
        }
        
        /**
         * Возвращаемся к игре
         */
        public void GameplayReturn() 
        {
            if (Origin.PreviousGameSpeed == 0)
            {
                GameSpeed.Value = 1;
            } else
            {
                GameSpeed.Value = Origin.PreviousGameSpeed;
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
            return Origin.CreateEntityID();
        }

        public void ProgressUp()
        {
            //TODO Создать Логарифмический расчет награды
            var delta = 50 / Mathf.Sqrt(2 * ProgressLevel.Value - 1);
            Progress.Value += Mathf.FloorToInt(delta);
            //Debug.Log("Progress delta = " + delta);
        }

        /**
         * CurrentWave отслеживает текущую волну, если номер превысил кол-во волн, то волны закончились 
         */
        public bool IsFinishWaves()
        {
            return CurrentWave.CurrentValue > Waves.Count;
        }
    }
}