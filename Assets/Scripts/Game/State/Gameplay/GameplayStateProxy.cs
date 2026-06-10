using System;
using System.Linq;
using Game.GamePlay.Classes;
using Game.GamePlay.Root;
using Game.State.Gameplay.Statistics;
using Game.State.Maps.Castle;
using Game.State.Maps.Grounds;
using Game.State.Maps.Heroes;
using Game.State.Maps.Mobs;
using Game.State.Maps.Rewards;
using Game.State.Maps.Roads;
using Game.State.Maps.Shots;
using Game.State.Maps.Skills;
using Game.State.Maps.Towers;
using Game.State.Maps.Warriors;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.State.Gameplay
{
    public class GameplayStateProxy : IDisposable
    {
        public readonly GameplayState Origin;

        //public readonly ReactiveProperty<int> GameSpeed;
        public readonly ReactiveProperty<int> Progress;
        public readonly ReactiveProperty<int> ProgressLevel;
        public readonly ReactiveProperty<long> SoftCurrency;
        public readonly ReactiveProperty<int> MapId;
        public readonly ReactiveProperty<bool> HasWaySecond;
        public readonly ReactiveProperty<int> CurrentWave;
        public readonly ReactiveProperty<int> UpdateCards;

        public readonly ReactiveProperty<float> TotalTimeInScene;
        public readonly ReactiveProperty<TypeGameplay> TypeGameplay;

        private float _previousGameSpeed;
        public int CountWaves;
        //Для отслеживания за игровой процесс

        public CastleEntity Castle;
        public HeroEntity Hero;
        public StatisticGame StatisticGame;

        public ObservableList<RewardEntityData> RewardEntities { get; } = new();
        public ObservableList<TowerEntity> Towers { get; } = new();
        public ObservableList<SkillEntity> Skills { get; } = new();

        public ObservableList<GroundEntity> Grounds { get; } = new();
        public ObservableList<RoadEntity> Way { get; } = new();
        public ObservableList<RoadEntity> WaySecond { get; } = new();
        public ObservableList<RoadEntity> WayDisabled { get; } = new();
        public ObservableList<WarriorEntity> Warriors { get; } = new();
        public ObservableList<MobEntity> Mobs { get; } = new();
        public ObservableList<MobEntity> BufferMobs { get; } = new();
        public ObservableList<MobEntity> SecondBufferMobs { get; } = new();

        public ReactiveProperty<SkillEntity> SkillOne;
        public ReactiveProperty<SkillEntity> SkillTwo;

        public readonly ReactiveProperty<bool> MapFinished = new(false);
        public ObservableList<ShotData> Shots { get; } = new();

        public ReadOnlyReactiveProperty<Vector2> GateWave { get; set; } // = new(Vector2.zero);
        public ReadOnlyReactiveProperty<Vector2> GateWaveSecond { get; set; } // = new(Vector2.zero);

        //Необходимо сохранить входные параметры игры для загрузки из сейва
        public GameplayEnterParams EnterParams => Origin.EnterParams;
        private DisposableBag _disposables = new();

        public GameplayStateProxy(GameplayState origin)
        {
            Origin = origin;
            if (origin.CastleData != null) Castle = new CastleEntity(origin.CastleData);
            if (origin.HeroData != null) Hero = new HeroEntity(origin.HeroData);
            
            StatisticGame = new StatisticGame(origin.StatisticGameData);
            _previousGameSpeed = Origin.GameSpeed;

            Progress = new ReactiveProperty<int>(origin.Progress);
            Progress.Subscribe(newProgress => origin.Progress = newProgress).AddTo(ref _disposables);
            ProgressLevel = new ReactiveProperty<int>(origin.ProgressLevel);
            ProgressLevel.Subscribe(newProgressLevel => origin.ProgressLevel = newProgressLevel)
                .AddTo(ref _disposables);

            SoftCurrency = new ReactiveProperty<long>(origin.SoftCurrency);
            SoftCurrency.Subscribe(newValue => origin.SoftCurrency = newValue).AddTo(ref _disposables);

            CurrentWave = new ReactiveProperty<int>(origin.CurrentWave);
            CurrentWave.Subscribe(newValue => origin.CurrentWave = newValue).AddTo(ref _disposables);

            UpdateCards = new ReactiveProperty<int>(origin.UpdateCards);
            UpdateCards.Subscribe(newValue => origin.UpdateCards = newValue).AddTo(ref _disposables);

            MapId = new ReactiveProperty<int>(origin.MapId);
            MapId.Subscribe(newValue => origin.MapId = newValue).AddTo(ref _disposables);

            TypeGameplay = new ReactiveProperty<TypeGameplay>(origin.TypeGameplay);
            TypeGameplay.Subscribe(newValue => origin.TypeGameplay = newValue).AddTo(ref _disposables);

            TotalTimeInScene = new ReactiveProperty<float>(0f);
            TotalTimeInScene.Subscribe(newValue => origin.TotalTimeInScene = newValue).AddTo(ref _disposables);

            SkillOne = new ReactiveProperty<SkillEntity>(new SkillEntity(origin.SkillOne));
            SkillTwo = new ReactiveProperty<SkillEntity>(new SkillEntity(origin.SkillTwo));

            //    GateWave = new ReactiveProperty<Vector2>(origin.GateWave);
            //    GateWave.Subscribe(newValue => origin.GateWave = newValue);

            //    GateWaveSecond = new ReactiveProperty<Vector2>(origin.GateWaveSecond);
            //    GateWaveSecond.Subscribe(newValue => origin.GateWaveSecond = newValue);

            HasWaySecond = new ReactiveProperty<bool>(origin.HasWaySecond);
            HasWaySecond.Subscribe(newValue => origin.HasWaySecond = newValue).AddTo(ref _disposables);

            //Награды
            origin.RewardEntities.ForEach(
                reward => RewardEntities.Add(reward)
            );
            RewardEntities.ObserveAdd().Subscribe(e => origin.RewardEntities.Add(e.Value))
                .AddTo(ref _disposables);

            InitMaps(origin);
        }

        private void InitMaps(GameplayState gameplayState)
        {
            //Земля
            gameplayState.Grounds.ForEach(
                groundOriginal => Grounds.Add(new GroundEntity(groundOriginal))
            );
            Grounds.ObserveAdd()
                .Subscribe(e => gameplayState.Grounds.Add(e.Value.Origin))
                .AddTo(ref this._disposables);

            Grounds.ObserveRemove()
                .Subscribe(e =>
                {
                    var removedMapState = gameplayState.Grounds.FirstOrDefault(b => b.UniqueId == e.Value.UniqueId);
                    gameplayState.Grounds.Remove(removedMapState);
                })
                .AddTo(ref _disposables);

            //Башни
            gameplayState.Towers.ForEach(
                towerOriginal => Towers.Add(new TowerEntity(towerOriginal))
            );
            Towers.ObserveAdd().Subscribe(e => gameplayState.Towers.Add(e.Value.Origin))
                .AddTo(ref _disposables);

            Towers.ObserveRemove().Subscribe(e =>
                {
                    var removedMapState = gameplayState.Towers.FirstOrDefault(b => b.UniqueId == e.Value.UniqueId);
                    gameplayState.Towers.Remove(removedMapState);
                })
                .AddTo(ref _disposables);
            //Навыки
            gameplayState.Skills.ForEach(
                skillOriginal => Skills.Add(new SkillEntity(skillOriginal))
            );
            Skills.ObserveAdd().Subscribe(e => gameplayState.Skills.Add(e.Value.Origin))
                .AddTo(ref _disposables);


            //Воины
            gameplayState.Warriors.ForEach(
                warriorOriginal => Warriors.Add(new WarriorEntity(warriorOriginal))
            );
            Warriors.ObserveAdd()
                .Subscribe(e => gameplayState.Warriors.Add(e.Value.Origin))
                .AddTo(ref _disposables);

            Warriors.ObserveRemove()
                .Subscribe(e =>
                {
                    var removedMapState = gameplayState.Warriors.FirstOrDefault(b => b.UniqueId == e.Value.UniqueId);
                    gameplayState.Warriors.Remove(removedMapState);
                })
                .AddTo(ref _disposables);

            //Дороги
            gameplayState.Way.ForEach(roadOriginal => Way.Add(new RoadEntity(roadOriginal)));
            Way.ObserveAdd().Subscribe(r => gameplayState.Way.Add(r.Value.Origin)).AddTo(ref _disposables);
            Way.ObserveRemove().Subscribe(r =>
            {
                var removedRoad = gameplayState.Way.FirstOrDefault(b => b.UniqueId == r.Value.UniqueId);
                gameplayState.Way.Remove(removedRoad);
            }).AddTo(ref _disposables);

            gameplayState.WaySecond.ForEach(roadOriginal => WaySecond.Add(new RoadEntity(roadOriginal)));
            WaySecond.ObserveAdd()
                .Subscribe(r => gameplayState.WaySecond.Add(r.Value.Origin))
                .AddTo(ref _disposables);
            WaySecond.ObserveRemove()
                .Subscribe(r =>
                {
                    var removedRoad = gameplayState.WaySecond.FirstOrDefault(b => b.UniqueId == r.Value.UniqueId);
                    gameplayState.WaySecond.Remove(removedRoad);
                }).AddTo(ref _disposables);

            gameplayState.WayDisabled.ForEach(roadOriginal => WayDisabled.Add(new RoadEntity(roadOriginal)));
            WayDisabled.ObserveAdd().Subscribe(r => gameplayState.WayDisabled.Add(r.Value.Origin)).AddTo(ref _disposables);
            WayDisabled.ObserveRemove().Subscribe(r =>
            {
                var removedRoad = gameplayState.WayDisabled.FirstOrDefault(b => b.UniqueId == r.Value.UniqueId);
                gameplayState.WayDisabled.Remove(removedRoad);
            }).AddTo(ref _disposables);

            //Список мобов на карте
            Mobs.ObserveAdd().Subscribe(v =>
            {
                var mobEntity = v.Value;
                mobEntity.IsDead
                    .Skip(1)
                    .Where(x => x)
                    .Subscribe(b => Mobs.Remove(mobEntity))
                    .AddTo(ref this._disposables);
            });

            Mobs.ObserveRemove().Subscribe(v =>
            {
                if (CurrentWave.CurrentValue == CountWaves && Mobs.Count == 0) MapFinished.OnNext(true);
            }).AddTo(ref _disposables);
        }


        /**
         * Ставим игру на паузу. Все объекты, которые зависят от скорости игры, подписываются на GameSpeed
         */
        public void SetPauseGame()
        {
            if (Time.timeScale != 0) _previousGameSpeed = Time.timeScale;
            Time.timeScale = 0;
        }

        /**
         * Возвращаемся к игре
         */
        public void GameplayReturn()
        {
            Time.timeScale = _previousGameSpeed == 0 ? 1 : _previousGameSpeed;
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
        }

        public float GetLastSpeedGame()
        {
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
                case 1:
                    newSpeed = 2;
                    break;
                case 2:
                    newSpeed = 4;
                    break;
                case 4:
                    newSpeed = 0.5f;
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
        }

        public bool IsInfinity()
        {
            return TypeGameplay.CurrentValue == GamePlay.Classes.TypeGameplay.Infinity;
        }

        public void SetTypeGameplay(TypeGameplay typeGameplay)
        {
            Origin.TypeGameplay = typeGameplay;
        }

        public void SetEnterParams(GameplayEnterParams gameplayEnterParams)
        {
            Origin.EnterParams = gameplayEnterParams;
        }

        public void Dispose()
        {
            _disposables.Dispose();
            Progress?.Dispose();
            ProgressLevel?.Dispose();
            SoftCurrency?.Dispose();
            MapId?.Dispose();
            HasWaySecond?.Dispose();
            CurrentWave?.Dispose();
            UpdateCards?.Dispose();
            TotalTimeInScene?.Dispose();
            TypeGameplay?.Dispose();
            SkillOne?.Dispose();
            SkillTwo?.Dispose();
            MapFinished?.Dispose();
            GateWave?.Dispose();
            GateWaveSecond?.Dispose();
            StatisticGame?.Dispose();
            Castle?.Dispose();
            Hero?.Dispose();
        }
        
    }
}