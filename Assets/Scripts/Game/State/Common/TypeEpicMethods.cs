using System;
using UnityEngine;

namespace Game.State.Common
{
    internal static class TypeEpicMethods
    {
        public static string GetString(this TypeEpic type)
        {
            return type switch
            {
                TypeEpic.Normal => "обычная",
                TypeEpic.Good => "хорошая",
                TypeEpic.Rare => "раритетная",
                TypeEpic.Epic => "эпическая",
                TypeEpic.EpicPlus => "эпическая +",
                TypeEpic.Legend => "легендарная",
                TypeEpic.LegendPlus => "легендарная +",
                TypeEpic.Cult => "культовая",
                TypeEpic.CultPlus => "культовая +",
                TypeEpic.Excellent => "превосходная",
                _ => ""
            };
        }

        public static TypeEpic Next(this TypeEpic type)
        {
            if (type == TypeEpic.Excellent) Debug.Log(" ERROR Next");
            return type switch
            {
                TypeEpic.Normal => TypeEpic.Good,
                TypeEpic.Good => TypeEpic.Rare,
                TypeEpic.Rare => TypeEpic.Epic,
                TypeEpic.Epic => TypeEpic.EpicPlus,
                TypeEpic.EpicPlus => TypeEpic.Legend,
                TypeEpic.Legend => TypeEpic.LegendPlus,
                TypeEpic.LegendPlus => TypeEpic.Cult,
                TypeEpic.Cult => TypeEpic.CultPlus,
                TypeEpic.CultPlus => TypeEpic.Excellent,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
        
        public static int MaxLevel(this TypeEpic type)
        {
            return type switch
            {
                TypeEpic.Normal => 10,
                TypeEpic.Good => 20,
                TypeEpic.Rare => 30,
                TypeEpic.Epic => 40,
                TypeEpic.EpicPlus => 50,
                TypeEpic.Legend => 60,
                TypeEpic.LegendPlus => 70,
                TypeEpic.Cult => 80,
                TypeEpic.CultPlus => 90,
                TypeEpic.Excellent => 100,
                _ => 0
            };
        }
        
        public static int Index(this TypeEpic type)
        {
            return type switch
            {
                TypeEpic.Normal => 0,
                TypeEpic.Good => 1,
                TypeEpic.Rare => 2,
                TypeEpic.Epic => 3,
                TypeEpic.EpicPlus => 4,
                TypeEpic.Legend => 5,
                TypeEpic.LegendPlus => 6,
                TypeEpic.Cult => 7,
                TypeEpic.CultPlus => 8,
                TypeEpic.Excellent => 9,
                _ => 0
            };
        }
    }
}