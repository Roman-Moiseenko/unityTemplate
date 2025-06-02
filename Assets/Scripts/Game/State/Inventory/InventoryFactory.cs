using System;
using Game.State.Inventory.TowerCard;
using Game.State.Inventory.TowerPlan;

namespace Game.State.Inventory
{
    public static class InventoryFactory
    {
        public static Inventory CreateInventory(InventoryData inventoryData)
        {
            
            //TODO Добавить новые типы инвентаря
            switch (inventoryData.TypeItem)
            {
                case InventoryType.TowerCard:
                    return new TowerCard.TowerCard(inventoryData as TowerCardData);
                case InventoryType.TowerPlan:
                    return new TowerPlan.TowerPlan(inventoryData as TowerPlanData);
                case InventoryType.SkillCard:
                //            return new SkillCard(inventoryData as ...);
                case InventoryType.SkillPlan:
                //          return new SkillPlan(inventoryData as ...);

                default:
                    throw new Exception($"Unsupported entity type: " + inventoryData.TypeItem);
            }
        }
    }
}