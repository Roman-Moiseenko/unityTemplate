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
                TypeChest.Epic => "Рубиновый сундук",
                TypeChest.Legend => "Алмазный сундук",
                null => "Нет места под сундук",
                _ => ""
            };
        }
        
        public static int FullHourOpening(this TypeChest type)
        {
            return type switch
            {
                TypeChest.Silver => 3,
                TypeChest.Gold => 8 ,
                TypeChest.Epic => 12,
                TypeChest.Legend => 24,
                _ => 0
            };
        }
        
        public static int GetRatioReward(this TypeChest type)
        {
            return type switch
            {
                TypeChest.Silver => 10,
                TypeChest.Gold => 12,
                TypeChest.Epic => 15,
                TypeChest.Legend => 20,
                _ => 0
            };
        }
        public static int GetIndex(this TypeChest type)
        {
            return type switch
            {
                TypeChest.Silver => 1,
                TypeChest.Gold => 2,
                TypeChest.Epic => 3,
                TypeChest.Legend => 4,
                _ => 0
            };
        }
        public static int GetRandom(this TypeChest type)
        {
            return type switch
            {
                TypeChest.Silver => 55,
                TypeChest.Gold => 30,
                TypeChest.Epic => 10,
                TypeChest.Legend => 5,
                _ => 0
            };
        }
    }
}