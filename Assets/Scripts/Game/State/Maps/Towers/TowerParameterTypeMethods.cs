namespace Game.State.Maps.Towers
{
    /**
     * Расширяем класс перечисления Типы параметров для башен
     */
    //TODO Добавить Локализацию
    internal static class TowerParameterTypeMethods
    {
        public static string GetString(this TowerParameterType type)
        {
            return type switch
            {
                TowerParameterType.Damage => "Урон в дальнем бою",
                TowerParameterType.Critical => "Шанс критического урона",
                TowerParameterType.Distance => "Дальность атаки",
                TowerParameterType.Health => "Здоровье",
                TowerParameterType.DamageArea => "Урон по области",
                TowerParameterType.Speed => "Скорость атаки",
                TowerParameterType.HighDamage => "Высокий урон",
                TowerParameterType.MiddleDamage => "Средний урон",
                TowerParameterType.MaxDistance => "Максимальная дистанция",
                TowerParameterType.MinDistance => "Минимальная дистанция",
                TowerParameterType.SlowingDown => "Замедление",
                _ => ""
            };
        }
    }
}