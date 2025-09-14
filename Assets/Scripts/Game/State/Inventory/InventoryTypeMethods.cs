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
    }
}