using System;

namespace Game.State.Inventory
{
    internal static class TypeEpicCardMethods
    {
        public static string GetString(this TypeEpicCard type)
        {
            return type switch
            {
                TypeEpicCard.Normal => "обычная",
                TypeEpicCard.Good => "хорошая",
                TypeEpicCard.Rare => "раритетная",
                TypeEpicCard.Epic => "эпическая",
                TypeEpicCard.EpicPlus => "эпическая +",
                TypeEpicCard.Legend => "легендарная",
                TypeEpicCard.LegendPlus => "легендарная +",
                TypeEpicCard.Cult => "культовая",
                TypeEpicCard.CultPlus => "культовая +",
                TypeEpicCard.Excellent => "превосходная",
                _ => ""
            };
        }

        public static TypeEpicCard Next(this TypeEpicCard type)
        {
            return type switch
            {
                TypeEpicCard.Normal => TypeEpicCard.Good,
                TypeEpicCard.Good => TypeEpicCard.Rare,
                TypeEpicCard.Rare => TypeEpicCard.Epic,
                TypeEpicCard.Epic => TypeEpicCard.EpicPlus,
                TypeEpicCard.EpicPlus => TypeEpicCard.Legend,
                TypeEpicCard.Legend => TypeEpicCard.LegendPlus,
                TypeEpicCard.LegendPlus => TypeEpicCard.Cult,
                TypeEpicCard.Cult => TypeEpicCard.CultPlus,
                TypeEpicCard.CultPlus => TypeEpicCard.Excellent,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
        
        public static int MaxLevel(this TypeEpicCard type)
        {
            return type switch
            {
                TypeEpicCard.Normal => 10,
                TypeEpicCard.Good => 20,
                TypeEpicCard.Rare => 30,
                TypeEpicCard.Epic => 40,
                TypeEpicCard.EpicPlus => 50,
                TypeEpicCard.Legend => 60,
                TypeEpicCard.LegendPlus => 70,
                TypeEpicCard.Cult => 80,
                TypeEpicCard.CultPlus => 90,
                TypeEpicCard.Excellent => 100,
                _ => 0
            };
        }
    }
}