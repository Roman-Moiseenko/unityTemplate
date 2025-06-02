using System.Collections.Generic;
using Game.GamePlay.Services;
using Game.Settings.Gameplay.Buildings;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Buildins
{
    public class BuildingViewModel
    {
    //    private readonly BuildingEntityProxy _buildingEntity;
        private readonly BuildingSettings _buildingSettings;
     //   private readonly BuildingsService _buildingsService;

        private readonly Dictionary<int, BuildingLevelSettings> _buildingLevelSettingsMap = new();

        public readonly int BuildingEntityId;
        public ReadOnlyReactiveProperty<int> Level { get; }
        public readonly string TypeId;
        
        public ReadOnlyReactiveProperty<Vector3Int> Position { get; }

    /*    public BuildingViewModel(
            BuildingEntityProxy buildingEntity,
            BuildingSettings buildingSettings,
            BuildingsService buildingsService
        )
        {
            BuildingEntityId = buildingEntity.Id;
            TypeId = buildingEntity.TypeId;
            Level = buildingEntity.Level;
            _buildingEntity = buildingEntity;
            _buildingSettings = buildingSettings;
            _buildingsService = buildingsService;

            foreach (var buildingLevelSettings in buildingSettings.LevelsSettings)
            {
                _buildingLevelSettingsMap[buildingLevelSettings.Level] = buildingLevelSettings;
            }

            Position = buildingEntity.Position;
        }
*/
        public BuildingLevelSettings GetLevelSettings(int level)
        {
            return _buildingLevelSettingsMap[level];
        }
    }
}