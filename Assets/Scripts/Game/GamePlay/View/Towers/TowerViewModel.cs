using System.Collections.Generic;
using Game.GamePlay.Services;
using Game.GamePlay.View.Frames;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State.Maps.Towers;
using Newtonsoft.Json;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Towers
{
    public class TowerViewModel : IMovingEntityViewModel
    {
        private readonly TowerEntity _towerEntity;
        public Dictionary<TowerParameterType, TowerParameterData> Parameters => _towerEntity.Parameters;
        private readonly List<TowerLevelSettings> _towerLevelSettings;
        private readonly TowersService _towerService;
        
        private readonly Dictionary<int, TowerLevelSettings> _towerLevelSettingsMap = new();

        public readonly int TowerEntityId;
        public ReactiveProperty<int> Level { get; set; }
        
        public readonly string ConfigId;
        private IMovingEntityViewModel _movingEntityViewModelImplementation;

        public ReactiveProperty<Vector2Int> Position { get; set; }
        public bool IsOnRoad => _towerEntity.IsOnRoad;
        public ReactiveProperty<bool> IsShot;

        public ReactiveProperty<Vector2> Direction;
        public ReactiveProperty<float> SpeedFire;
        
        
        public TowerViewModel(
            TowerEntity towerEntity,
            List<TowerLevelSettings> towerLevelSettings,
            TowersService towerService
        )
        {
            _towerEntity = towerEntity;
            IsShot = towerEntity.IsShot;
            _towerLevelSettings = towerLevelSettings;
            _towerService = towerService;
            TowerEntityId = towerEntity.UniqueId;
            ConfigId = towerEntity.ConfigId;
            Level = towerEntity.Level;
            if (towerLevelSettings != null)
            {
                foreach (var towerLevelSetting in towerLevelSettings)
                {
                    _towerLevelSettingsMap[towerLevelSetting.Level] = towerLevelSetting;
                }
            }

            Direction = towerEntity.PrepareShot;
            Position = towerEntity.Position;
            SpeedFire = new ReactiveProperty<float>();
            if (towerEntity.Parameters.TryGetValue(TowerParameterType.Speed, out var towerSpeed))
            {
                towerService.GameSpeed.Where(x => x != 0).Subscribe(v =>
                {
                    SpeedFire.Value = towerSpeed.Value / v; 
                });
            }
            else
            {
                SpeedFire.Value = 0;
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
            if (_towerEntity.Parameters.TryGetValue(TowerParameterType.MinDistance, out var min))
            {
                radius.y = min.Value;
            }
            
            if (_towerEntity.Parameters.TryGetValue(TowerParameterType.MaxDistance, out var max))
            {
                radius.x = max.Value;
            }
            
            if (_towerEntity.Parameters.TryGetValue(TowerParameterType.Distance, out var parameter))
            {
                radius.x = parameter.Value;
            }
            
            //TODO Если к башне применен параметр Высота (+дистанции) то вычисляем radius.z = %% от radius.x

            return radius;
        }
    }
}