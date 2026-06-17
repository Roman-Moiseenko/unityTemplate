using System;
using Game.State.Inventory.HeroCards;
using Game.State.Inventory.SkillCards;
using Game.State.Inventory.SkillPlans;
using Game.State.Inventory.TowerCards;
using Game.State.Inventory.TowerPlans;

namespace Game.State.Inventory.Common
{
    public static class InventoryFactory
    {
        public static InventoryItem CreateInventory(InventoryItemData inventoryItemData)
        {
            //TODO Добавить новые типы инвентаря
            switch (inventoryItemData.TypeItem)
            {
                case InventoryType.TowerCard:
                    return new TowerCard(inventoryItemData as TowerCardData);
                case InventoryType.TowerPlan:
                    return new TowerPlan(inventoryItemData as TowerPlanData);
                case InventoryType.SkillCard:
                    return new SkillCard(inventoryItemData as SkillCardData);
                case InventoryType.SkillPlan:
                    return new SkillPlan(inventoryItemData as SkillPlanData);
                case InventoryType.HeroCard:
                    return new HeroCard(inventoryItemData as HeroCardData);
                
                default:
                    throw new Exception($"Unsupported entity type: " + inventoryItemData.TypeItem);
            }
        }
    }
}