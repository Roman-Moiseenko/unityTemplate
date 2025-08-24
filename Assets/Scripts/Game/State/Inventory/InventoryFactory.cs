using System;
using Game.State.Inventory.SkillCards;
using Game.State.Inventory.TowerCards;
using Game.State.Inventory.TowerPlans;

//using Game.State.Inventory.TowerPlan;

namespace Game.State.Inventory
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
                          //return new SkillPlan(inventoryItemData as SkillPlanData);

                default:
                    throw new Exception($"Unsupported entity type: " + inventoryItemData.TypeItem);
            }
        }

        public static InventoryItem Create(InventoryItemData inventoryItemData)
        {
            var type = inventoryItemData.GetType();

            if (type == typeof(TowerCardData)) return new TowerCard(inventoryItemData as TowerCardData);
            if (type == typeof(TowerPlanData)) return new TowerPlan(inventoryItemData as TowerPlanData);


            throw new Exception($"Unsupported entity type: " + type);

        }
    }
}