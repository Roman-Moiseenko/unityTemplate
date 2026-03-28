namespace Game.State.Maps.Skills
{
    internal static class SkillParameterTypeMethods
    {
        public static string GetString(this SkillParameterType type)
        {
            return type switch
            {
                SkillParameterType.DPS => "УВС",
                SkillParameterType.Damage => "Урон",
                SkillParameterType.Cooldown => "Перезарядка",
                SkillParameterType.Duration => "Длительность",
                SkillParameterType.Range => "Радиус",
                SkillParameterType.Health => "Макс.здоровье",
                SkillParameterType.Warriors=> "Кол-во бойцов",
                SkillParameterType.Cells=> "Кол-во клеток",
                SkillParameterType.Slow => "Замедление",
                SkillParameterType.Stun => "Длит. оглушения",
                SkillParameterType.Targets => "Кол-во целей",
                SkillParameterType.Healing => "Исцеление",
                _ => ""
            };
        }


        public static bool IsDamage(this SkillParameterType type)
        {
            return type is SkillParameterType.Damage or SkillParameterType.DPS;
        }

        public static string GetMeasure(this SkillParameterType type)
        {
            return type switch
            {
                SkillParameterType.Slow => "%",
                SkillParameterType.Healing => "%",
                SkillParameterType.Stun => " сек",
                SkillParameterType.Cooldown => " сек",
                SkillParameterType.Duration => " сек",
                _ => ""
            };
        }
    }
}