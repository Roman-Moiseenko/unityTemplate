namespace Game.State.Maps.Mobs
{
    internal static class MobTypeMethods
    {
        public static string GetString(this MobType type)
        {
            return type switch
            {

                MobType.Infantry => "Пехота",
                MobType.HeavyInfantry => "Тяжелая пехота",
                //    FireBlaster,
                //   AirSniper,
                //    Wrecker,
                MobType.Boss => "Босс",
                
                _ => ""
            };
        }
    }
}