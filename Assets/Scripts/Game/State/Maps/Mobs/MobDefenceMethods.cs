using System;

namespace Game.State.Maps.Mobs
{
    internal static class MobDefenceMethods
    {
        public static string GetString(this MobDefence type)
        {
            return type switch
            {
                MobDefence.Fast => "Быстрые",
                MobDefence.Advanced => "Усиленные",
                MobDefence.Elemental => "Стихия",
                MobDefence.Support => "Поддержка",
                _ => ""
            };
        }

        public static MobDefence Next(this MobDefence type)
        {
            return type switch
            {
                MobDefence.Fast => MobDefence.Elemental,
                MobDefence.Advanced => MobDefence.Fast,
                MobDefence.Elemental => MobDefence.Advanced,
                _ => throw new Exception("Ошибк")
            };
        }
        
        public static MobDefence Previous(this MobDefence type)
        {
            return type switch
            {
                MobDefence.Fast => MobDefence.Advanced,
                MobDefence.Advanced => MobDefence.Elemental,
                MobDefence.Elemental => MobDefence.Fast,
                _ => throw new Exception("Ошибк")
            };
        }
    }
}