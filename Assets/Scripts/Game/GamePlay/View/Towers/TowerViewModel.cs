using System.Collections.Generic;
using Game.GamePlay.Services;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State.Maps.Towers;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Towers
{
    public class TowerViewModel
    {
        private readonly TowerEntity _towerEntity;
        private readonly TowerSettings _towerSettings;
        private readonly TowersService _towerService;

        private readonly Dictionary<int, TowerLevelSettings> _towerLevelSettingsMap = new();

        public readonly int TowerEntityId;
        public ReadOnlyReactiveProperty<int> Level { get; }
        public ReadOnlyReactiveProperty<int> EpicLevel { get; }
        public readonly string ConfigId;
        
        public ReadOnlyReactiveProperty<Vector2Int> Position { get; }

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
            Level = towerEntity.Level; //TODO не нужно ...
            EpicLevel = towerEntity.EpicLevel;
            

            foreach (var towerLevelSettings in towerSettings.Levels)
            {
                _towerLevelSettingsMap[towerLevelSettings.Level] = towerLevelSettings;
            }

            Position = towerEntity.Position;
        }

        public TowerLevelSettings GetLevelSettings(int level)
        {
            return _towerLevelSettingsMap[level];
        }
    }
}