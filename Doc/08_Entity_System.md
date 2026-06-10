# 8. Entity System — система сущностей карты

## Концепция

Каждая сущность на карте (башня, моб, дорога, земля, замок, воин, скилл, герой) представлена парой классов:
- **EntityData** (POCO) — сериализуемые данные (хранятся в GameplayState)
- **Entity** (Proxy) — реактивная обёртка с `ReactiveProperty` для полей

## Все типы сущностей

| Сущность | EntityData | Entity | ObservableList в Proxy |
|----------|-----------|--------|----------------------|
| Башня | `TowerEntityData` | `TowerEntity` | `Towers` |
| Моб | `MobEntityData` | `MobEntity` | `Mobs`, `BufferMobs`, `SecondBufferMobs` |
| Дорога | `RoadEntityData` | `RoadEntity` | `Way`, `WaySecond`, `WayDisabled` |
| Земля | `GroundEntityData` | `GroundEntity` | `Grounds` |
| Замок | `CastleEntityData` | `CastleEntity` | Отдельное поле |
| Герой | `HeroEntityData` | `HeroEntity` | Отдельное поле |
| Воин | `WarriorEntityData` | `WarriorEntity` | `Warriors` |
| Скилл | `SkillEntityData` | `SkillEntity` | `Skills`, `SkillOne`, `SkillTwo` |
| Выстрел | `ShotData` (не Entity) | — | `Shots` |
| Награда | `RewardEntityData` | — | `RewardEntities` |

## Пример: TowerEntity

### TowerEntityData (POCO)

```csharp
public class TowerEntityData
{
    public int UniqueId;
    public string ConfigId;                // ID конфига в настройках
    public int GameplayLevel;              // Уровень башни
    public Vector2Int Position;           // Позиция на карте
    public Vector2Int Placement;          // Плейсмент (привязка к дороге)
}
```

### TowerEntity (Proxy)

```csharp
public class TowerEntity : IDisposable
{
    public TowerEntityData Origin;
    
    public int UniqueId => Origin.UniqueId;
    public string ConfigId => Origin.ConfigId;
    
    public ReactiveProperty<int> GameplayLevel;
    public ReactiveProperty<Vector2Int> Position;
    public ReactiveProperty<Vector2Int> Placement;
    
    // Вычисляемые поля (из настроек, не сохраняются)
    public bool IsOnRoad;                  // На дороге ли башня
    public TypeTarget TypeTarget;          // Тип цели (Air/Ground/Universal)
    public TypeDefence Defence;            // Тип защиты
    public bool IsMultiShot;               // Множественный выстрел
    public bool IsSingleTarget;            // Одиночная цель
    public bool IsPlacement;               // Размещаемая (не атакующая)
    public float SpeedShot;                // Скорость выстрела
    public Dictionary<ParameterType, ParameterData> Parameters;  // Параметры башни
}
```

## Пример: MobEntity

```csharp
public class MobEntity : IDisposable
{
    public MobEntityData Origin;
    
    public int UniqueId => Origin.UniqueId;
    public string ConfigId => Origin.ConfigId;
    
    public ReactiveProperty<Vector2> Position;       // Текущая позиция на поле
    public ReactiveProperty<int> Health;              // Текущее здоровье
    public ReactiveProperty<bool> IsDead;             // Мёртв ли
    public ReactiveProperty<bool> IsFinish;           // Дошёл ли до замка
    public ReactiveProperty<TypeDebuff> Debuff;       // Текущий дебафф
    public ReactiveProperty<float> Speed;             // Скорость передвижения
    
    public int MaxHealth;                             // Максимальное здоровье (из настроек)
    public TypeMob TypeMob;                           // Тип моба (наземный/воздушный)
    public TypeTarget TypeTarget;                     // Тип цели для башен
    public bool IsTarget;                             // Является ли целью
    public int IndexRoadWay;                          // Индекс точки на пути
    public List<RoadPoint> RoadPoints;                // Точки маршрута
    
    public DebuffData DebuffData;                     // Данные дебаффа
}
```

## Пример: RoadEntity

```csharp
public class RoadEntity : IDisposable
{
    public RoadEntityData Origin;
    
    public int UniqueId => Origin.UniqueId;
    public string ConfigId => Origin.ConfigId;
    public Vector2Int Position => Origin.Position;
    public ReactiveProperty<TypeRoad> TypeRoad;
    public ReactiveProperty<RoadDirection> Direction;
    
    public bool IsWaySecond => Origin.IsWaySecond;   // Принадлежит второму пути
    public bool IsDisabled => Origin.IsDisabled;     // Отключена
    public bool IsSpawn;                              // Точка спавна мобов
    public bool IsBase;                               // Возле замка
    public List<MobEntity> MobsOnRoad;               // Мобы на этой дороге
}
```

## Пример: CastleEntity

```csharp
public class CastleEntity : IDisposable
{
    public CastleEntityData Origin;
    
    public ReactiveProperty<int> Health;
    public ReactiveProperty<int> MaxHealth;
    public ReactiveProperty<bool> IsDestroyed;
    
    public Vector2Int Position;                      // Позиция замка
}
```

## Пример: WarriorEntity

```csharp
public class WarriorEntity : IDisposable
{
    public WarriorEntityData Origin;
    
    public ReactiveProperty<Vector2> Position;
    public ReactiveProperty<int> Health;
    public ReactiveProperty<bool> IsDead;
    
    public string ConfigId => Origin.ConfigId;
    public TypeWarrior TypeWarrior;                  // Тип воина
}
```

## Принцип создания Entity

```csharp
// Внутри CommandHandler
public bool Handle(CommandPlaceTower command)
{
    // 1. Создаём POCO-данные
    var data = new TowerEntityData
    {
        UniqueId = _gameplayState.CreateEntityID(),
        ConfigId = command.ConfigId,
        Position = command.Position,
        GameplayLevel = 1
    };
    
    // 2. Оборачиваем в Entity (реактивная обёртка)
    var entity = new TowerEntity(data);
    
    // 3. Добавляем в ObservableList (автосинхронизация с Origin)
    _gameplayState.Towers.Add(entity);
    
    return true;
}
```

## Принцип удаления Entity

```csharp
// Внутри CommandHandler
public bool Handle(CommandDeleteTower command)
{
    var tower = _gameplayState.Towers.FirstOrDefault(t => t.UniqueId == command.TowerId);
    if (tower == null) return false;
    
    // Удаление из ObservableList → автосинхронизация с Origin
    _gameplayState.Towers.Remove(tower);
    
    // Dispose реактивной обёртки
    tower.Dispose();
    
    return true;
}
```

---

*Читайте далее: [09_Settings.md](09_Settings.md)*
