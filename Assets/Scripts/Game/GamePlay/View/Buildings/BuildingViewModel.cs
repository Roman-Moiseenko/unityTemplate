using System.Collections.Generic;
using Game.GamePlay.Services;
using Game.Settings.Gameplay.Buildings;
using Game.Settings.Gameplay.Entities.Buildings;
using Game.State.Mergeable.Buildings;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Buildings
{
    public class BuildingViewModel
    {
        private readonly BuildingEntity _buildingEntity;
        private readonly BuildingSettings _buildingSettings;
        private readonly BuildingsService _buildingsService;

        private readonly Dictionary<int, BuildingLevelSettings> _buildingLevelSettingsMap = new();

        public readonly int BuildingEntityId;
        public ReadOnlyReactiveProperty<int> Level { get; }
        public readonly string ConfigId;
        
        public ReadOnlyReactiveProperty<Vector2Int> Position { get; }

        public BuildingViewModel(
            BuildingEntity buildingEntity,
            BuildingSettings buildingSettings,
            BuildingsService buildingsService
        )
        {
            BuildingEntityId = buildingEntity.UniqueId;
            ConfigId = buildingEntity.ConfigId;
            Level = buildingEntity.Level;
            _buildingEntity = buildingEntity;
            _buildingSettings = buildingSettings;
            _buildingsService = buildingsService;

            foreach (var buildingLevelSettings in buildingSettings.Levels)
            {
                _buildingLevelSettingsMap[buildingLevelSettings.Level] = buildingLevelSettings;
            }

            Position = buildingEntity.Position;
        }

        public BuildingLevelSettings GetLevelSettings(int level)
        {
            return _buildingLevelSettingsMap[level];
        }
    }
}