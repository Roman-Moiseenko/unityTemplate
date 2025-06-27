using System;
using Game.State.Inventory.TowerCards;

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
                    return new TowerCards.TowerCard(inventoryItemData as TowerCardData);
                /*case InventoryType.TowerPlan:
                    return new TowerPlan.TowerPlan(inventoryData as TowerPlanData);*/
                case InventoryType.SkillCard:
                //            return new SkillCard(inventoryData as ...);
                case InventoryType.SkillPlan:
                //          return new SkillPlan(inventoryData as ...);

                default:
                    throw new Exception($"Unsupported entity type: " + inventoryItemData.TypeItem);
            }
        }
    }
}