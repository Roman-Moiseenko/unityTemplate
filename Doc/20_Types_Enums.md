# 20. Типы и вспомогательные перечисления

## Основные перечисления

### Типы сущностей

```csharp
public enum TypeGameplay
{
    Levels,         // Обычный уровень
    Infinity,       // Бесконечный режим
    Event,          // Событие
    Resume          // Продолжение сохранённой сессии
}

public enum TypeTarget
{
    Air,            // Воздушная цель
    Ground,         // Наземная цель
    Universal       // Универсальная (и воздух, и земля)
}

public enum TypeMob
{
    Ground,         // Наземный
    Air             // Воздушный
}

public enum TypeDefence
{
    None,           // Нет защиты
    Light,          // Лёгкая броня
    Heavy           // Тяжёлая броня
}
```

### Типы предметов инвентаря

```csharp
public enum TypeInventoryItem
{
    TowerCard,      // Карточка башни
    SkillCard,      // Карточка скилла
    HeroCard,       // Карточка героя
    TowerPlan,      // План улучшения башни
    SkillPlan,      // План улучшения скилла
    Chest,          // Сундук
    Currency        // Валюта
}

public enum TypeChest
{
    Common,         // Обычный сундук
    Epic,           // Эпический сундук
    Legendary,      // Легендарный сундук
    None            // Нет сундука
}
```

### Типы дорог

```csharp
public enum TypeRoad
{
    Horizontal,     // Горизонтальная дорога
    Vertical,       // Вертикальная дорога
    TurnLeft,       // Поворот налево
    TurnRight,      // Поворот направо
    Cross           // Перекрёсток
}

public enum RoadDirection
{
    Up,             // Вверх
    Down,           // Вниз
    Left,           // Влево
    Right,          // Вправо
    None            // Нет направления
}
```

### Типы скиллов

```csharp
public enum TypeSkill
{
    Damage,         // Урон по области
    Slow,           // Замедление
    Stun,           // Оглушение
    Heal,           // Лечение замка
    Buff            // Бафф башен
}

public enum TypeDebuff
{
    None,           // Нет дебаффа
    Slow,           // Замедление
    Stun,           // Оглушение
    Poison          // Яд (периодический урон)
}
```

### Типы воинов

```csharp
public enum TypeWarrior
{
    Melee,          // Ближний бой
    Archer,         // Лучник
    Repair          // Ремонтник (чинит башни)
}
```

### Параметры башен

```csharp
public enum ParameterType
{
    Damage,             // Урон
    SpeedAttack,        // Скорость атаки
    Range,              // Дальность
    CritChance,         // Шанс крита
    CritMultiplier,     // Множитель крита
    SplashRadius,       // Радиус разлёта
    DamageOverTime,     // Периодический урон
    SlowPercent,        // Процент замедления
    StunDuration,       // Длительность оглушения
    CountTarget         // Количество целей
}

public class ParameterData
{
    public ParameterType Type;
    public float BaseValue;
    public float Increment;     // Прирост за уровень
}
```

### Состояния загрузки

```csharp
public enum LoadingState
{
    None,           // Не начато
    Loading,        // Загружается
    Loaded,         // Загружено
    Error           // Ошибка
}
```

## Вспомогательные классы

### SceneEnterParams / SceneExitParams

```csharp
public class SceneEnterParams
{
    public string SceneName;  // Имя сцены для перехода
}

public class SceneExitParams
{
    public SceneEnterParams TargetSceneEnterParams;  // Куда перейти
}
```

### GameplayBoosters

```csharp
public class GameplayBoosters
{
    public int DamagePercent;
    public int CritChance;
    public int SpeedAttack;
    public bool IsDoubleReward;
}
```

### DebuffData

```csharp
public class DebuffData
{
    public TypeDebuff Type;
    public float Duration;
    public float Value;          // Сила эффекта (процент замедления, урон в секунду)
    public float TimeRemaining;  // Оставшееся время
}
```

### ShotData

```csharp
public class ShotData
{
    public int UniqueId;
    public int TowerEntityId;    // ID башни, которая стреляет
    public int MobEntityId;      // ID моба-цели
    public float Damage;         // Урон
    public Vector2 Position;     // Позиция выстрела
    public bool IsCrit;          // Критический удар
    public TypeDefence Defence;  // Тип защиты цели (для расчёта)
}
```

---

*Читайте далее: [21_Conclusion.md](21_Conclusion.md)*
