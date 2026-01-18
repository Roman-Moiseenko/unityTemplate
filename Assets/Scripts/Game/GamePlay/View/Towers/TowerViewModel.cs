using System;
using System.Collections.Generic;
using Game.GamePlay.Services;
using Game.GamePlay.View.Frames;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State.Maps.Mobs;
using Game.State.Maps.Shots;
using Game.State.Maps.Towers;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Towers
{
    public class TowerViewModel : IMovingEntityViewModel
    {
        public TowerEntity TowerEntity { get; }

        public Dictionary<TowerParameterType, TowerParameterData> Parameters => TowerEntity.Parameters;
        private List<TowerLevelSettings> _towerLevelSettings;
        private readonly TowersService _towerService;
        
        private readonly Dictionary<int, TowerLevelSettings> _towerLevelSettingsMap = new();

        public readonly int TowerEntityId;
        public ReactiveProperty<int> Level { get; set; }
        
        public readonly string ConfigId;
        private IMovingEntityViewModel _movingEntityViewModelImplementation;

        public ReactiveProperty<Vector2Int> Position { get; set; }
        public bool IsOnRoad => TowerEntity.IsOnRoad;
        public ReactiveProperty<bool> IsShot;

        public ReactiveProperty<Vector3> Direction = new();
        public float SpeedFire = 0f;
        
        public ReactiveProperty<int> NumberModel = new(0);
        public float SpeedShot => TowerEntity.SpeedShot;
        //public ReactiveProperty<Vector3> Direction = new();

        public IObservableCollection<MobEntity> Targets => TowerEntity.Targets;
        public TowerViewModel(
            TowerEntity towerEntity,
            List<TowerLevelSettings> towerLevelSettings,
            TowersService towerService
        )
        {
            _towerService = towerService;
            IsShot = towerEntity.IsShot;
            TowerEntityId = towerEntity.UniqueId;
            ConfigId = towerEntity.ConfigId;
            Level = towerEntity.Level;
            //Direction = towerEntity.PrepareShot;
            Position = towerEntity.Position;
            //SpeedFire = new ReactiveProperty<float>();
            //GameSpeed = towerService.GameSpeed;
            TowerEntity = towerEntity;
            
            
            _towerLevelSettings = towerLevelSettings;
            if (towerLevelSettings != null)
            {
                foreach (var towerLevelSetting in towerLevelSettings)
                {
                    _towerLevelSettingsMap[towerLevelSetting.Level] = towerLevelSetting;
                }
            }
            if (towerEntity.Parameters.TryGetValue(TowerParameterType.Speed, out var towerSpeed))
            {
               // towerService.GameSpeed.Where(x => x != 0).Subscribe(v =>
               // {
                    SpeedFire = towerSpeed.Value;// / v; 
                //});
            }

            Level.Subscribe(level =>
            {
                //TODO Смена модели и/или материал
                NumberModel.Value = level switch
                {
                    1 or 2 => 1,
                    3 or 4 => 2,
                    5 or 6 => 3,
                    _ => throw new Exception("Неизвестный уровень")
                };
                
            });
            
        }

        public void UpdateParameters(List<TowerLevelSettings> towerLevelSettings)
        {
            _towerLevelSettings = towerLevelSettings;
            if (towerLevelSettings != null)
            {
                foreach (var towerLevelSetting in towerLevelSettings)
                {
                    _towerLevelSettingsMap[towerLevelSetting.Level] = towerLevelSetting;
                }
            }
            if (TowerEntity.Parameters.TryGetValue(TowerParameterType.Speed, out var towerSpeed))
            {
               // _towerService.GameSpeed.Where(x => x != 0).Subscribe(v =>
                //{
                    SpeedFire = towerSpeed.Value;// / v; 
               // });
            }
            else
            {
                SpeedFire = 0;
            }
        }
        public TowerLevelSettings GetLevelSettings(int level)
        {
            return _towerLevelSettingsMap[level];
        }

        public bool IsPosition(Vector2 position)
        {
            float delta = 0.5f; //Половина ширины клетки
            int _x = Position.CurrentValue.x;
            int _y = Position.CurrentValue.y;
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
            if (TowerEntity.Parameters.TryGetValue(TowerParameterType.MinDistance, out var min))
            {
                radius.y = min.Value;
            }
            
            if (TowerEntity.Parameters.TryGetValue(TowerParameterType.MaxDistance, out var max))
            {
                radius.x = max.Value;
            }
            
            if (TowerEntity.Parameters.TryGetValue(TowerParameterType.Distance, out var parameter))
            {
                radius.x = parameter.Value;
            }
            
            //TODO Если к башне применен параметр Высота (+дистанции) то вычисляем radius.z = %% от radius.x

            return radius;
        }

        public string GetNameModel()
        {
            return Level.CurrentValue switch
            {
                1 or 2 => "Base",
                3 or 4 => "Upgrade",
                5 or 6 => "Finish",
                _ => throw new Exception("Неизвестный уровень")
            };
        }
        
        public void RemoveTarget(MobEntity mobEntity)
        {
            TowerEntity.RemoveTarget(mobEntity);
        }

        public void SetDirection(Vector2Int direction)
        {
            Direction.Value = new Vector3(direction.x, 0, direction.y);
        }
    }
}