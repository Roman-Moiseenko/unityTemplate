using System;

namespace Game.State.Inventory
{
    internal static class InventoryTypeMethods
    {
        public static bool IsAccumulation(this InventoryType type)
        {
            return type switch
            {
                InventoryType.TowerCard => false,
                InventoryType.SkillCard => false,
                _ => true
            };
        }
        
                public static int GetChanceRandom(this InventoryType type)
                {
                    //Шанс % = 100 / x
                    return type switch
                    {
                        InventoryType.TowerCard => 10, //10%
                        InventoryType.SkillCard => 10,
                        InventoryType.TowerPlan => 4, //20%
                        InventoryType.SkillPlan => 4,
                        InventoryType.SoftCurrency => 2, //50%
                        InventoryType.HardCurrency => 15, 
                        InventoryType.HeroCard => 25, //4%
                        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                    };
                }
        
    }
}