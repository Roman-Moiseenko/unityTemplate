using Game.State.Inventory.Common;

namespace Game.State.Inventory.SkillPlans
{
    /**
     * Чертеж карты башни
     */
    public class SkillPlanData : InventoryItemData
    {
        public override bool Accumulation => true;
        public override InventoryType TypeItem => InventoryType.SkillPlan;
    }
}