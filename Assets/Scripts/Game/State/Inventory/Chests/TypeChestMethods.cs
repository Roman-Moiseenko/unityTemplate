using System;

namespace Game.State.Inventory.Chests
{
    internal static class TypeChestMethods
    {
        public static string GetString(this TypeChest? type)
        {
            return type switch
            {
                TypeChest.Silver => "Серебрянный сундук",
                TypeChest.Gold => "Золотой сундук",
                TypeChest.Ruby => "Рубиновый сундук",
                TypeChest.Diamond => "Алмазный сундук",
                null => "Нет места под сундук",
                _ => ""
            };
        }
        
        public static int TimeOut(this TypeChest type)
        {
            return type switch
            {
                TypeChest.Silver => 3,
                TypeChest.Gold => 8 ,
                TypeChest.Ruby => 12,
                TypeChest.Diamond => 24,
                _ => 0
            };
        }
    }
}