using Game.State.Maps.Towers;

namespace Game.State.Parameter
{
    /**
     * Расширяем класс перечисления Типы параметров для башен
     */
    //TODO Добавить Локализацию
    internal static class ParameterTypeMethods
    {
        public static string GetString(this ParameterType type)
        {
            return type switch
            {
                ParameterType.Damage => "Урон",
                ParameterType.Critical => "Шанс крита",
                ParameterType.Distance => "Дальность атаки",
                ParameterType.Health => "Здоровье",
                ParameterType.DamageArea => "Урон по области",
                ParameterType.Speed => "Частота атаки",
                ParameterType.HighDamage => "Высокий урон",
                ParameterType.MiddleDamage => "Средний урон",
                ParameterType.MaxDistance => "Максимальная дистанция",
                ParameterType.MinDistance => "Минимальная дистанция",
                ParameterType.SlowingDown => "Замедление",
                ParameterType.Warriors => "Кол-во бойцов",
                ParameterType.Range => "Диапазон",

                ParameterType.DPS => "УВС",
                ParameterType.Cooldown => "Перезарядка",
                ParameterType.Duration => "Длительность",
                ParameterType.Radius => "Радиус",
                ParameterType.Cells => "Кол-во клеток",
                ParameterType.Slow => "Замедление",
                ParameterType.Stun => "Длит. оглушения",
                ParameterType.Targets => "Кол-во целей",
                ParameterType.Healing => "Исцеление",
                _ => ""
            };
        }

        public static bool IsDamage(this ParameterType type)
        {
            return type is ParameterType.Damage
                or ParameterType.DamageArea
                or ParameterType.HighDamage
                or ParameterType.MiddleDamage
                or ParameterType.DPS;
        }

        public static string GetMeasure(this ParameterType type)
        {
            return type switch
            {
                ParameterType.Critical => "%",
                ParameterType.Speed => " сек",
                ParameterType.SlowingDown => "%",


                ParameterType.Slow => "%",
                ParameterType.Healing => "%",
                ParameterType.Stun => " сек",
                ParameterType.Cooldown => " сек",
                ParameterType.Duration => " сек",

                _ => ""
            };
        }
    }
}