using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.GamePlay.Commands.WarriorCommands;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.TowerStates;
using Game.GamePlay.Services;
using Game.GamePlay.View.Frames;
using Game.GamePlay.View.Mobs;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State.Maps.Mobs;
using Game.State.Maps.Shots;
using Game.State.Maps.Towers;
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
        protected readonly GameplayStateProxy _gameplayState;
        protected readonly TowersService _towersService;
        protected readonly TowerEntity _towerEntity;
        public bool IsPlacement => _towerEntity.IsPlacement; 
        public Dictionary<TowerParameterType, TowerParameterData> Parameters => _towerEntity.Parameters;
        public readonly int UniqueId;
        public ReactiveProperty<int> Level { get; set; }
        public readonly string ConfigId;
        public ReactiveProperty<Vector2Int> Position { get; set; }
        public ReactiveProperty<Vector3> PositionMap = new();
        public bool IsOnRoad => _towerEntity.IsOnRoad;
        public ReactiveProperty<Vector3> Direction = new();
        
        public ReactiveProperty<int> NumberModel = new(0);
        public float SpeedShot => _towerEntity.SpeedShot;
        public ReactiveProperty<bool> FinishEffectLevelUp = new(false);
        public ReactiveProperty<bool> ShowArea = new(false);


        
        //Флаг для передачи в Панели подтверждения из различных состояния
    //    public ReactiveProperty<bool> IsConfirmationState = new(true);
        
    
        public TowerViewModel(
            TowerEntity towerEntity,
            GameplayStateProxy gameplayState,
            TowersService towersService,
            FsmTower fsmTower
        )
        {
            _gameplayState = gameplayState;
            _towersService = towersService;
            _towerEntity = towerEntity;   
            
            UniqueId = towerEntity.UniqueId;
            ConfigId = towerEntity.ConfigId;
            Level = towerEntity.Level;
            Position = towerEntity.Position;

            Position.Subscribe(v => PositionMap.Value = new Vector3(v.x, 0, v.y));
            
            
            Level.Subscribe(level =>
            { //Смена модели
                NumberModel.Value = level switch
                {
                    1 or 2 => 1,
                    3 or 4 => 2,
                    5 or 6 => 3,
                    _ => throw new Exception("Неизвестный уровень")
                };
            });
            fsmTower?.Fsm.StateCurrent.Subscribe(state =>
            {
                if (state.GetType() == typeof(FsmTowerNone)) ShowArea.Value = false;
                if (state.GetType() == typeof(FsmTowerSelected) && fsmTower.GetTowerViewModel().UniqueId == UniqueId)
                    ShowArea.Value = true;
            });
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
            if (_towerEntity.IsPlacement) return new Vector3(5, 5, 1); //Scale для модели
            
            var radius = Vector3.zero; //Zero для башен без области
            if (_towerEntity.Parameters.TryGetValue(TowerParameterType.MinDistance, out var min))
                radius.y = min.Value;
            if (_towerEntity.Parameters.TryGetValue(TowerParameterType.MaxDistance, out var max))
                radius.x = max.Value;
            if (_towerEntity.Parameters.TryGetValue(TowerParameterType.Distance, out var parameter))
                radius.x = parameter.Value;
            
            //TODO Если к башне применен параметр Высота (+дистанции) то вычисляем radius.z = %% от radius.x
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
            Level?.Dispose();
            Position?.Dispose();
        }
    }
}