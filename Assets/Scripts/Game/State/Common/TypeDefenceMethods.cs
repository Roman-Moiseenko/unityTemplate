using System;

namespace Game.State.Common
{
    internal static class TypeDefenceMethods
    {
        public static string GetString(this TypeDefence type)
        {
            return type switch
            {
                TypeDefence.Fast => "Быстрые",
                TypeDefence.Advanced => "Усиленные",
                TypeDefence.Elemental => "Стихия",
                TypeDefence.Support => "Поддержка",
                _ => ""
            };
        }

        public static TypeDefence Next(this TypeDefence type)
        {
            return type switch
            {
                TypeDefence.Fast => TypeDefence.Elemental,
                TypeDefence.Advanced => TypeDefence.Fast,
                TypeDefence.Elemental => TypeDefence.Advanced,
                _ => throw new Exception("Ошибка")
            };
        }
        
        public static TypeDefence Previous(this TypeDefence type)
        {
            return type switch
            {
                TypeDefence.Fast => TypeDefence.Advanced,
                TypeDefence.Advanced => TypeDefence.Elemental,
                TypeDefence.Elemental => TypeDefence.Fast,
                _ => throw new Exception("Ошибка")
            };
        }
    }
}