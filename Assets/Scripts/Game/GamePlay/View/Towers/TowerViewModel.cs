using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.GamePlay.Commands.WarriorCommands;
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
    public class TowerViewModel : IMovingEntityViewModel
    {
        private readonly GameplayStateProxy _gameplayState;
        private readonly TowersService _towersService;
        private readonly TowerEntity _towerEntity;
        public bool IsPlacement => _towerEntity.IsPlacement; 
        public Dictionary<TowerParameterType, TowerParameterData> Parameters => _towerEntity.Parameters;
        public readonly int UniqueId;
        public ReactiveProperty<int> Level { get; set; }
        public readonly string ConfigId;
        public ReactiveProperty<Vector2Int> Position { get; set; }
        public ReactiveProperty<Vector3> PositionMap = new();
        public bool IsOnRoad => _towerEntity.IsOnRoad;
        public ReactiveProperty<bool> IsShot;
        public ReactiveProperty<Vector3> Direction = new();
        public float Speed = 0f;
        public ReactiveProperty<int> NumberModel = new(0);
        public float SpeedShot => _towerEntity.SpeedShot;
        public bool IsMultiShot => _towerEntity.IsMultiShot;

        public bool IsSingleTarget => _towerEntity.IsSingleTarget;
      //  private readonly Dictionary<int, TowerLevelSettings> _towerLevelSettingsMap = new();
        private IMovingEntityViewModel _movingEntityViewModelImplementation;

        public ReactiveProperty<bool> IsBusy = new(false);
        public ReactiveProperty<float> MaxDistance = new(0f);
        public float MinDistance = 0f;

        public ObservableDictionary<int, MobViewModel> MobTargets = new();
        
        public TowerViewModel(
            TowerEntity towerEntity,
            GameplayStateProxy gameplayState,
            TowersService towersService
        )
        {
            _gameplayState = gameplayState;
            _towersService = towersService;
            _towerEntity = towerEntity;   
            
            IsShot = towerEntity.IsShot;
            UniqueId = towerEntity.UniqueId;
            ConfigId = towerEntity.ConfigId;
            Level = towerEntity.Level;
            Position = towerEntity.Position;

            Position.Subscribe(v => PositionMap.Value = new Vector3(v.x, 0, v.y));

            if (towerEntity.Parameters.TryGetValue(TowerParameterType.Speed, out var towerSpeed))
                Speed = towerSpeed.Value;

            if (towerEntity.Parameters.TryGetValue(TowerParameterType.MinDistance, out var towerMinDistance))
                MinDistance = towerMinDistance.Value;
            
            Level.Subscribe(level =>
            {
                
                if (towerEntity.Parameters.TryGetValue(TowerParameterType.Distance, out var towerDistance))
                    MaxDistance.Value = towerDistance.Value;
                if (towerEntity.Parameters.TryGetValue(TowerParameterType.MaxDistance, out var towerMaxDistance))
                    MaxDistance.Value = towerMaxDistance.Value;
                
                //Смена модели
                NumberModel.Value = level switch
                {
                    1 or 2 => 1,
                    3 or 4 => 2,
                    5 or 6 => 3,
                    _ => throw new Exception("Неизвестный уровень")
                };
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
            Position.Value = position;
        }

        public Vector2Int GetPosition()
        {
            return Position.CurrentValue;
        }

        public Vector3 GetRadius()
        {
            var radius = new Vector3(0, 0, 0);
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
        
        public void SetTarget(MobViewModel viewModel)
        {
            if (MobTargets.TryGetValue(viewModel.UniqueId, out var value)) return;
            MobTargets.TryAdd(viewModel.UniqueId, viewModel);
        }

        public void RemoveTarget(MobViewModel mobBinderViewModel)
        {
            MobTargets.Remove(mobBinderViewModel.UniqueId);
        }

        public void ClearTargets()
        {
            foreach (var (key, mobViewModel) in MobTargets.ToArray())
            {
                RemoveTarget(mobViewModel);
            }
        }
        
        /**
         * Башня наносящая урон
         */
        public void SetDamageAfterShot(MobViewModel mobViewModel)
        {
            var shot = _towerEntity.GetShotParameters(mobViewModel.Defence);
            shot.MobEntityId = mobViewModel.UniqueId;
            _gameplayState.Shots.Add(shot); 
        }

        /**
         * Башня призывающая воинов
         */
        public bool IsDeadAllWarriors()
        {
            return _towersService.IsDeadAllWarriors(_towerEntity);
        }
        public void AddWarriorsTower()
        {
            _towersService.AddWarriorsTower(_towerEntity);
        }

        /**
         * Проверяем на совместимость Башни и моба для нанесения урона
         */
        public bool IsTargetForDamage(bool mobIsFly)
        {
            switch (_towerEntity.TypeEnemy)
            {
                case TowerTypeEnemy.Universal:
                case TowerTypeEnemy.Air when mobIsFly:
                case TowerTypeEnemy.Ground when !mobIsFly:
                    return true;
                default:
                    return false;
            }
        }
    }
}