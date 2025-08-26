namespace Game.State.Inventory.TowerPlans
{
    /**
     * Чертеж карты башни
     */
    public class TowerPlanData : InventoryItemData
    {
        public override bool Accumulation => true;
        public override InventoryType TypeItem => InventoryType.TowerPlan;
    }
}