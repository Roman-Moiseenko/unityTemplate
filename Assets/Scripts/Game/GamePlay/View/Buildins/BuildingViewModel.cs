using Game.GamePlay.Services;
using Game.State.Entities.Buildings;

namespace Game.GamePlay.View.Buildins
{
    public class BuildingViewModel
    {
        private readonly BuildingEntityProxy _buildingEntity;
        private readonly BuildingsService _buildingsService;

        public BuildingViewModel(BuildingEntityProxy buildingEntity, BuildingsService buildingsService)
        {
            _buildingEntity = buildingEntity;
            _buildingsService = buildingsService;
        }
    }
}