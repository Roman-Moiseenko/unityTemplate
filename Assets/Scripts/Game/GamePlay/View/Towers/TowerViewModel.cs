using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DI;
using Game.GamePlay.Commands.WarriorCommands;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.TowerStates;
using Game.GamePlay.Root;
using Game.GamePlay.Services;
using Game.GamePlay.View.Frames;
using Game.GamePlay.View.Mobs;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State;
using Game.State.Common;
using Game.State.Gameplay;
using Game.State.Inventory;
using Game.State.Maps.Mobs;
using Game.State.Maps.Shots;
using Game.State.Maps.Towers;
using Game.State.Parameters;
using Game.State.Research;
using Game.State.Root;
using MVVM.CMD;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Towers
{
    /**
     * Базовый клас для моделей башни, напрямую используется только во Frame
     * Вся логика (Атака, Воины, Баф/Дебаф) определена в дочерних классах
     */
    public class TowerViewModel : IMovingEntityViewModel, IDisposable
    {
        protected GameplayStateProxy GameplayState;
        protected TowersService TowersService;
        protected readonly TowerEntity TowerEntity;
        protected GameplayBoosters GameplayBoosters;
        private readonly DIContainer _container;
        public bool IsPlacement => TowerEntity.IsPlacement;
        public Dictionary<ParameterType, ParameterData> Parameters => TowerEntity.Parameters;
        public readonly int UniqueId;
        public ReactiveProperty<int> Level { get; set; }
        public readonly string ConfigId;
        public ReactiveProperty<Vector2Int> Position { get; set; }
        public readonly ReactiveProperty<Vector3> PositionMap = new();
        public bool IsOnRoad => TowerEntity.IsOnRoad;
        public readonly ReactiveProperty<Vector3> Direction = new();

        public readonly ReactiveProperty<int> NumberModel = new(0);
        public float SpeedShot { get; private set; }
        public TypeEpic EpicLevel { get; set; }
        public ReactiveProperty<bool> FinishEffectLevelUp = new(false);
        public ReactiveProperty<bool> ShowArea = new(false);
        public TypeTarget TypeTarget => TowerEntity.TypeTarget;
        protected DisposableBag _disposables = new();

        //Отображение на карте для обмена башнями

        public ReactiveProperty<bool> ShowReplaceTag = new(false);
        public ReactiveProperty<bool> ShowSelected = new(false);

        //Флаг для передачи в Панели подтверждения из различных состояния
        //    public ReactiveProperty<bool> IsConfirmationState = new(true);


        public TowerViewModel(
            TowerEntity towerEntity,
            DIContainer container
            //GameplayStateProxy gameplayState,
            //TowersService towersService,
            //FsmTower fsmTower
        )
        {
            var fsmTower = container.Resolve<FsmTower>();
            GameplayState = container.Resolve<IGameStateProvider>().GameplayState;
            TowersService = container.Resolve<TowersService>();
            // container.Resolve<>
            GameplayBoosters = container.Resolve<GameplayEnterParams>().GameplayBoosters;
            _container = container;
            
            
            TowerEntity = towerEntity;
            UniqueId = towerEntity.UniqueId;
            ConfigId = towerEntity.ConfigId;
            Level = towerEntity.GameplayLevel;
            Position = towerEntity.Position;
            Position
                .Subscribe(v =>
                {
                    PositionMap.Value = new Vector3(v.x, 0, v.y);
//                    Debug.Log("Subscribe = " + v);
                })
                .AddTo(ref _disposables);
            SpeedShot = TowerEntity.SpeedShot;
            //Есть бустер на скорострельность
            var busters = TowersService.TowerBoosters[ConfigId];
            if (busters.TryGetValue(ParameterType.Speed, out float value))
            {
                SpeedShot -= SpeedShot * value / 100f;
            }
            var availableTowers = TowersService.GetAvailableTowers();
            EpicLevel = availableTowers[TowerEntity.ConfigId];        
            
            Level.Subscribe(level =>
            { //Смена модели
                NumberModel.Value = level switch
                {
                    1 or 2 => 1,
                    3 or 4 => 2,
                    5 or 6 => 3,
                    _ => throw new Exception("Неизвестный уровень")
                };
            }).AddTo(ref _disposables);

            fsmTower?.Fsm.StateCurrent.Subscribe(state =>
            {
                if (state.GetType() == typeof(FsmTowerNone)) ShowArea.Value = false;
                if (state.GetType() == typeof(FsmTowerSelected) && fsmTower.GetTowerViewModel().UniqueId == UniqueId)
                    ShowArea.Value = true;
            }).AddTo(ref _disposables);
        }

        public bool IsPosition(Vector2 position)
        {
            const float delta = 0.5f; //Половина ширины клетки
            var _x = Position.CurrentValue.x;
            var _y = Position.CurrentValue.y;
            if ((position.x >= _x - delta && position.x <= _x + delta) &&
                (position.y >= _y - delta && position.y <= _y + delta))
                return true;
            return false;
        }

        public void SetPosition(Vector2Int position)
        {
            //
        }

        public Vector2Int GetPosition()
        {
            return Position.CurrentValue;
        }

        public Vector3 GetAreaRadius()
        {
            if (TowerEntity.IsPlacement) return new Vector3(5, 5, 1); //Scale для модели

            var radius = Vector3.zero; //Zero для башен без области
            if (TowerEntity.Parameters.TryGetValue(ParameterType.MinDistance, out var min))
                radius.y = min.Value;
            if (TowerEntity.Parameters.TryGetValue(ParameterType.MaxDistance, out var max))
                radius.x = max.Value;
            if (TowerEntity.Parameters.TryGetValue(ParameterType.Distance, out var parameter))
                radius.x = parameter.Value;

            //MAINDO Если к башне применен параметр Высота (+дистанции) то вычисляем radius.z = %% от radius.x

            //Если есть бустер на дистанцию, то добавить в radius.z 
            if (TowersService.TowerBoosters.TryGetValue(ConfigId, out var boosters))
            {
                if (boosters.TryGetValue(ParameterType.Distance, out var distance))
                    radius.z += radius.x * distance / 100;    
            }
            

            return radius;
        }

        public void SetDirection(Vector2Int direction)
        {
            Direction.Value = new Vector3(direction.x, 0, direction.y);
        }

        public virtual void Dispose()
        {
            PositionMap?.Dispose();
            Direction?.Dispose();
            NumberModel?.Dispose();
            FinishEffectLevelUp?.Dispose();
            ShowArea?.Dispose();
            _disposables.Dispose();
        }
        public void Selected()
        {
            ShowSelected.Value = true;
        }
        public void UnSelected()
        {
            if (ShowSelected.CurrentValue) ShowSelected.Value = false;
        }
    }
}