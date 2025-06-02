using Game.State.Maps.Towers;

namespace Game.State.Inventory.TowerPlan
{
    public class TowerPlan: Inventory
    {
        public TowerType TowerType { get; }

        public TowerPlan(TowerPlanData data) : base(data)
        {
            TowerType = data.TowerType;
        }
    }
}