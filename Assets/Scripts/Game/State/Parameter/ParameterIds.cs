namespace Game.State.Parameter
{
    /// <summary>
    /// Строковые константы typeId для всех используемых параметров.
    /// Служат типизированными ключами для Dictionary и соответствуют
    /// ParameterDefinition.Id из JSON-конфига с сервера.
    /// </summary>
    public static class ParameterIds
    {
        // --- Общие (Tower + Skill + Hero) ---
        /// <summary>Урон (базовый)</summary>
        public const string Damage = "Damage";
        
        // --- Общие (Tower + Hero) ---
        /// <summary>Дальность атаки (базовый)</summary>
        public const string Distance = "Distance";
        /// <summary>Шанс критического удара</summary>
        public const string Critical = "Critical";
        /// <summary>Частота атаки</summary>
        public const string Speed = "Speed";
        
        // --- Общие (Tower + Skill) ---
        /// <summary>Количество бойцов</summary>
        public const string Warriors = "Warriors";
        /// <summary>Здоровье</summary>
        public const string Health = "Health";
 
        
        // --- Только башни (Tower) ---
        /// <summary>Урон по области</summary>
        public const string DamageArea = "DamageArea";
        /// <summary>Максимальная дистанция</summary>
        public const string MaxDistance = "MaxDistance";
        /// <summary>Минимальная дистанция</summary>
        public const string MinDistance = "MinDistance";
        /// <summary>Замедление цели (в %)</summary>
        public const string SlowingDown = "SlowingDown";
        /// <summary>Высокий урон</summary>
        public const string HighDamage = "HighDamage";
        /// <summary>Средний урон</summary>
        public const string MiddleDamage = "MiddleDamage";
        /// <summary>Диапазон</summary>
        public const string Range = "Range";
        
        // --- Только навыки (Skill) ---
        /// <summary>Урон в секунду (УВС)</summary>
        public const string DPS = "DPS";
        /// <summary>Радиус</summary>
        public const string Radius = "Radius";
        /// <summary>Перезарядка</summary>
        public const string Cooldown = "Cooldown";
        /// <summary>Длительность действия</summary>
        public const string Duration = "Duration";
        /// <summary>Количество клеток</summary>
        public const string Cells = "Cells";
        /// <summary>Замедление (в %)</summary>
        public const string Slow = "Slow";
        /// <summary>Длительность оглушения</summary>
        public const string Stun = "Stun";
        /// <summary>Количество целей</summary>
        public const string Targets = "Targets";
        /// <summary>Исцеление (в %)</summary>
        public const string Healing = "Healing";
    }
}
