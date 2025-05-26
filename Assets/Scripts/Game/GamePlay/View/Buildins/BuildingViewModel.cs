using Game.GamePlay.Services;
using Game.State.Entities.Buildings;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Buildins
{
    public class BuildingViewModel
    {
        private readonly BuildingEntityProxy _buildingEntity;
        private readonly BuildingsService _buildingsService;

        public readonly int BuildingEntityId;
        public ReadOnlyReactiveProperty<Vector3Int> Position { get; }

        public BuildingViewModel(BuildingEntityProxy buildingEntity, BuildingsService buildingsService)
        {
            BuildingEntityId = buildingEntity.Id;
            _buildingEntity = buildingEntity;
            _buildingsService = buildingsService;

            Position = buildingEntity.Position;
        }
    }
}