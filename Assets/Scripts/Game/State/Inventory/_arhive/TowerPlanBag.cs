using Game.State.Inventory.TowerPlans;

namespace Game.State.Inventory.TowerCards
{
    public class TowerPlanBag : InventoryBag<TowerPlan, TowerPlanData>
    {
        public override bool Accumulation => true;
        
        public TowerPlanBag(InventoryBagData<TowerPlanData> bagData) : base(bagData)
        {
        }


    }
}