using DI;
using Game.MainMenu.Services;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State.Inventory.TowerCards;
using Game.State.Inventory.TowerPlans;
using R3;

namespace Game.MainMenu.View.ScreenInventory.TowerPlans
{
    public class TowerPlanViewModel
    {
        public TowerPlan TowerPlan => _towerPlanEntity;
        public int IdTowerPlan => _towerPlanEntity.UniqueId;
        public string ConfigId => _towerPlanEntity.ConfigId;
        public ReadOnlyReactiveProperty<long> Amount => _towerPlanEntity.Amount;
        public readonly TowerSettings TowerSettings;

        private readonly DIContainer _container;
        private readonly TowerPlan _towerPlanEntity;

        public TowerPlanViewModel(TowerPlan towerPlanEntity, TowerSettings towerSettings, DIContainer container)
        {
            _towerPlanEntity = towerPlanEntity;
            TowerSettings = towerSettings;
            _container = container;
        }


        public void RequestOpenPopupTowerPlan()
        {
            _container.Resolve<Subject<TowerPlanViewModel>>().OnNext(this);
        }
    }
}