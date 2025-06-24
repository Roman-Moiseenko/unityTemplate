using System.Collections.Generic;
using Game.GamePlay.Services;
using Game.GamePlay.View.Frames;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State.Maps.Towers;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Towers
{
    public class TowerViewModel : IMovingEntityViewModel
    {
        private readonly TowerEntity _towerEntity;
        private readonly TowerSettings _towerSettings;
        private readonly TowersService _towerService;
        
        private readonly Dictionary<int, TowerLevelSettings> _towerLevelSettingsMap = new();

        public readonly int TowerEntityId;
        public ReactiveProperty<int> Level { get; set; }
        
        public readonly string ConfigId;
        private IMovingEntityViewModel _movingEntityViewModelImplementation;

        public ReactiveProperty<Vector2Int> Position { get; set; }

        public TowerViewModel(
            TowerEntity towerEntity,
            TowerSettings towerSettings,
            TowersService towerService
        )
        {
            _towerEntity = towerEntity;
            _towerSettings = towerSettings;
            _towerService = towerService;
            TowerEntityId = towerEntity.UniqueId;
            ConfigId = towerEntity.ConfigId;
            Level = towerEntity.Level;
            if (towerSettings != null)
            {
                foreach (var towerLevelSettings in towerSettings.Levels)
                {
                    _towerLevelSettingsMap[towerLevelSettings.Level] = towerLevelSettings;
                }
            }
            
            Position = towerEntity.Position;
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
    }
}