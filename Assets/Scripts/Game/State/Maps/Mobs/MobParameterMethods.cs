namespace Game.State.Maps.Mobs
{
    internal static class MobParameterMethods
    {
        public static string GetString(this MobParameter type)
        {
            return type switch
            {
                MobParameter.Health => "Здоровье",
                MobParameter.Damage => "Урон",
                MobParameter.Burn => "Ожог",
                MobParameter.Poison => "Яд",
                MobParameter.Bite => "Укус",
                _ => ""
            };
        }
        
        public static bool IsDamage(this MobParameter type)
        {
            return type switch
            {
                MobParameter.Burn => true,
                MobParameter.Poison => true,
                MobParameter.Bite => true,
                _ => false
            };
        }
    }
}