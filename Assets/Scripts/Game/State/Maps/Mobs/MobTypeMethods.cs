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
        
        public static MobDefence GetDefence(this MobType type)
        {
            return type switch
            {

                MobType.Infantry => MobDefence.Fast,
                MobType.HeavyInfantry => MobDefence.Advanced,
                //    FireBlaster,
                //   AirSniper,
                //    Wrecker,
                MobType.Boss => MobDefence.Advanced,
                
                _ => MobDefence.Fast
            };
        }
    }
}