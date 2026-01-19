using System;
using System.Linq;
using Game.GamePlay.Classes;
using Game.State.Entities;
using Game.State.Maps.Castle;
using Game.State.Maps.Grounds;
using Game.State.Maps.Rewards;
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

        //public readonly ReactiveProperty<int> GameSpeed;
        public readonly ReactiveProperty<int> Progress;
        public readonly ReactiveProperty<int> ProgressLevel;
        public readonly ReactiveProperty<long> SoftCurrency;
        public readonly ReactiveProperty<int> MapId;
        public readonly ReactiveProperty<int> CurrentWave;
        public readonly ReactiveProperty<int> UpdateCards;
        private float _previousGameSpeed;
        public readonly ReactiveProperty<int> KillMobs;
        public readonly ReactiveProperty<TypeGameplay> TypeGameplay;
        
        //Для отслеживания за игровой процесс
       // public readonly ReactiveProperty<bool> IsFinishWave = new(false);

        public CastleEntity Castle;
        
        public ObservableList<RewardEntityData> RewardEntities { get; } = new(); 
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
            _previousGameSpeed = Origin.GameSpeed;
         //   GameSpeed = new ReactiveProperty<int>(origin.GameSpeed);
       //     GameSpeed.Subscribe(newSpeed =>
      //      {
       //         origin.GameSpeed = newSpeed;
       //     });
            
            Progress = new ReactiveProperty<int>(origin.Progress);
            Progress.Subscribe(newProgress => origin.Progress = newProgress);
            ProgressLevel = new ReactiveProperty<int>(origin.ProgressLevel);
            ProgressLevel.Subscribe(newProgressLevel => origin.ProgressLevel = newProgressLevel);

            SoftCurrency = new ReactiveProperty<long>(origin.SoftCurrency);
            SoftCurrency.Subscribe(newValue => origin.SoftCurrency = newValue);

            CurrentWave = new ReactiveProperty<int>(origin.CurrentWave);
            CurrentWave.Subscribe(newValue => origin.CurrentWave = newValue);

            UpdateCards = new ReactiveProperty<int>(origin.UpdateCards);
            UpdateCards.Subscribe(newValue => origin.UpdateCards = newValue);

            MapId = new ReactiveProperty<int>(origin.MapId);
            MapId.Subscribe(newValue => origin.MapId = newValue);

            KillMobs = new ReactiveProperty<int>(origin.KillMobs);
            KillMobs.Subscribe(newValue => origin.KillMobs = newValue);

            TypeGameplay = new ReactiveProperty<TypeGameplay>(origin.TypeGameplay);
            TypeGameplay.Subscribe(newValue => origin.TypeGameplay = newValue);
            
            //Награды
            origin.RewardEntities.ForEach(
                reward => RewardEntities.Add(reward)
            );
            RewardEntities.ObserveAdd().Subscribe(e => origin.RewardEntities.Add(e.Value));
            
            InitMaps(origin);
        }

        private void InitMaps(GameplayState gameplayState)
        {
            //Земля
            gameplayState.Grounds.ForEach(
                groundOriginal => Grounds.Add(new GroundEntity(groundOriginal))
            );
            Grounds.ObserveAdd().Subscribe(e => gameplayState.Grounds.Add(e.Value.Origin));

            Grounds.ObserveRemove().Subscribe(e =>
            {
                var removedMapState = gameplayState.Grounds.FirstOrDefault(b => b.UniqueId == e.Value.UniqueId);
                gameplayState.Grounds.Remove(removedMapState);
            });
            
            //Башни
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
            _previousGameSpeed = Time.timeScale; //GameSpeed.Value;
            Time.timeScale = 0;
            //GameSpeed.Value = 0;
        }
        
        /**
         * Возвращаемся к игре
         */
        public void GameplayReturn()
        {
            //GameSpeed.Value 
            //Debug.Log("_previousGameSpeed = " + _previousGameSpeed);
            Time.timeScale = _previousGameSpeed == 0 ? 1 : _previousGameSpeed;
            //Debug.Log("Time.timeScale  = " + Time.timeScale );
        }

        public void SetSkillSpeed()
        {
            _previousGameSpeed = Time.timeScale; //GameSpeed.Value;
            SetGameSpeed(1);
        }
        
        private void SetGameSpeed(float newSpeed)
        {
            Time.timeScale = newSpeed;
            Origin.GameSpeed = newSpeed; //Запоминаем скорость
            //if (newSpeed == GameSpeed.Value) return;
            //GameSpeed.Value = newSpeed;
        }

        public float GetLastSpeedGame()
        {
            //GameSpeed.CurrentValue;
            return Mathf.Max(Time.timeScale, _previousGameSpeed);
        }

        public float GetCurrentSpeed()
        {
            return Time.timeScale == 0 ? _previousGameSpeed : Time.timeScale;
        }

        public float SetNextSpeed()
        {
            var newSpeed = 1f;
            switch (Time.timeScale)
            {
                case 1: newSpeed = 2;
                    break;
                case 2: newSpeed = 4;
                    break;
                case 4: newSpeed = 0.5f;
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

        public bool IsInfinity()
        {
            return TypeGameplay.CurrentValue == GamePlay.Classes.TypeGameplay.Infinity;
        }

        public void SetTypeGameplay(TypeGameplay typeGameplay)
        {
            Origin.TypeGameplay = typeGameplay;
        }
        
        
    }
}