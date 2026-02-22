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
                TowerParameterType.Damage => "Урон",
                TowerParameterType.Critical => "Шанс крита",
                TowerParameterType.Distance => "Дальность атаки",
                TowerParameterType.Health => "Здоровье",
                TowerParameterType.DamageArea => "Урон по области",
                TowerParameterType.Speed => "Частота атаки",
            //    TowerParameterType.HighDamage => "Высокий урон",
              //  TowerParameterType.MiddleDamage => "Средний урон",
                TowerParameterType.MaxDistance => "Максимальная дистанция",
                TowerParameterType.MinDistance => "Минимальная дистанция",
                TowerParameterType.SlowingDown => "Замедление",
                TowerParameterType.Warriors => "Кол-во бойцов",
                _ => ""
            };
        }

        public static string GetMeasure(this TowerParameterType type)
        {
            return type switch
            {
                TowerParameterType.Critical => "%",
                TowerParameterType.Speed => " сек",
                TowerParameterType.SlowingDown => "%",
                
                //TowerParameterType.Damage => "",
                //TowerParameterType.Distance => "",
                //TowerParameterType.Health => "",
                //TowerParameterType.DamageArea => "",
                //    TowerParameterType.HighDamage => "",
                //  TowerParameterType.MiddleDamage => "",
                //TowerParameterType.MaxDistance => "",
                //TowerParameterType.MinDistance => "",
                //TowerParameterType.Warriors => "",
                _ => ""
            };
            
        }
    }
}