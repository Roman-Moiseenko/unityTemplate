namespace Game.State.Parameters
{
    public enum ParameterType
    {
        Damage,
        DamageArea,
        Distance,
        MaxDistance,
        MinDistance,
        Speed,
        Critical,
        Health,
        SlowingDown,
        Range,
        Warriors,
        MiddleDamage,
        HighDamage,
        
        //Навыки
        DPS, //УВС
        Cooldown, //Перезарядка
        Duration, //Длительность
        Radius, //Радиус
        Cells, //Кол-во клеток
        Slow, //Замедление в %
        Stun, //Длительность оглушения
        Targets, //Кол-во целей
        Healing, //Исцеление в %
    }
}