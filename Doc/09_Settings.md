Да# 9. Settings Provider (настройки игры)

## Интерфейс ISettingsProvider

```csharp
public interface ISettingsProvider
{
    GameSettings GameSettings { get; }
    ApplicationSettings ApplicationSettings { get; }
    Observable<LoadingState> LoadGameSettings();
}
```

Реализация: `SettingsProviderWeb` — загружает настройки с сервера через UnityWebRequest.

## Структура GameSettings

```csharp
public class GameSettings
{
    public MapsSettings MapsSettings;              // Настройки всех карт
    public TowersSettings TowersSettings;          // Настройки всех башен
    public SkillsSettings SkillsSettings;          // Настройки всех скиллов
    public HeroesSettings HeroesSettings;          // Настройки героев
    public CastleInitialSettings CastleInitialSettings;  // Начальные настройки замка
    public InventoryInitialSettings InventoryInitialSettings;  // Начальный инвентарь
    public MobsSettings MobsSettings;              // Настройки врагов
    public DateTime DateVersion;                   // Версия данных
}
```

## MapsSettings — настройки карт

```csharp
public class MapsSettings
{
    public List<MapData> Maps;                     // Список карт
    public List<string> GroundConfigIds;           // ID конфигов земли
    public List<string> RoadConfigIds;             // ID конфигов дорог
}

public class MapData
{
    public string ConfigId;                        // ID карты
    public string Name;                            // Название
    public int MapNumber;                          // Номер карты
    public Vector2Int CastlePosition;              // Позиция замка
    public int Width;                              // Ширина карты
    public int Height;                             // Высота карты
    public List<WaveSetting> Waves;                // Настройки волн
    public InitialStateSettings InitialState;      // Начальное состояние (дороги, башни)
    public MapRewardSetting Reward;                // Награды
    public List<InitialTowerSettings> InitialTowers; // Начальные башни
}
```

### WaveSetting — настройка волны

```csharp
public class WaveSetting
{
    public int Number;                             // Номер волны
    public float Timer;                            // Время до старта
    public List<MobSetting> Mobs;                  // Мобы в волне
}

public class MobSetting
{
    public string ConfigId;                        // ID моба
    public int Count;                              // Количество
    public float Interval;                         // Интервал спавна
}
```

### MapRewardSetting — награды карты

```csharp
public class MapRewardSetting
{
    public List<RewardOnWave> RewardOnWave;        // Награды за волны
    public TypeChest ChestWin;                     // Сундук при победе
    public TypeChest ChestLose;                    // Сундук при поражении
}

public class RewardOnWave
{
    public int WaveNumber;                         // Номер волны
    public long SoftCurrency;                      // Золото
    public List<RewardItem> Items;                 // Предметы
}
```

## TowersSettings — настройки башен

```csharp
public class TowersSettings
{
    public List<TowerSetting> AllTowers;           // Все башни
}

public class TowerSetting
{
    public string ConfigId;                        // ID башни
    public string Name;                            // Название
    public TypeTarget TypeTarget;                  // Тип цели (Air/Ground/Universal)
    public TypeDefence Defence;                    // Тип защиты
    public bool IsMultiShot;                       // Множественный выстрел
    public bool IsSingleTarget;                    // Одиночная цель
    public bool IsPlacement;                       // Размещаемая
    public float SpeedShot;                        // Скорость выстрела
    public string SpriteId;                        // ID спрайта
    public List<LevelSettings> GameplayLevels;     // Уровни
}

public class LevelSettings
{
    public int Level;                              // Уровень
    public List<ParameterData> Parameters;         // Параметры (урон, скорость, дистанция)
}

public class ParameterData
{
    public ParameterType Type;                     // Тип параметра
    public float BaseValue;                        // Базовое значение
    public float Increment;                        // Прирост за уровень
}
```

## SkillsSettings — настройки скиллов

```csharp
public class SkillsSettings
{
    public List<SkillSetting> AllSkills;           // Все скиллы
}

public class SkillSetting
{
    public string ConfigId;                        // ID скилла
    public string Name;                            // Название
    public TypeSkill TypeSkill;                    // Тип скилла
    public float Cooldown;                         // Перезарядка
    public float Damage;                           // Урон
    public float Radius;                           // Радиус
    public TypeTarget TypeTarget;                  // Тип цели
    public string SpriteId;                        // ID спрайта
}
```

## MobsSettings — настройки врагов

```csharp
public class MobsSettings
{
    public List<MobSettingData> AllMobs;           // Все мобы
}

public class MobSettingData
{
    public string ConfigId;                        // ID моба
    public string Name;                            // Название
    public TypeMob TypeMob;                        // Тип (наземный/воздушный)
    public TypeTarget TypeTarget;                  // Тип цели
    public int Health;                             // Здоровье
    public float Speed;                            // Скорость
    public long Reward;                            // Награда за убийство
    public int Damage;                             // Урон замку
    public TypeDebuff Debuff;                      // Дебафф
    public string SpriteId;                        // ID спрайта
}
```

## HeroesSettings — настройки героев

```csharp
public class HeroesSettings
{
    public List<HeroSettingData> Heroes;           // Все герои
}

public class HeroSettingData
{
    public string ConfigId;                        // ID героя
    public string Name;                            // Название
    public int Health;                             // Здоровье
    public string SpriteId;                        // ID спрайта
    public List<HeroBuffData> Buffs;               // Баффы героя
}
```

## ApplicationSettings — настройки приложения

```csharp
public class ApplicationSettings
{
    public string ApiUrl;                          // URL API сервера
    public string Version;                         // Версия приложения
    public int TargetFrameRate;                    // Целевой FPS
}
```

---

*Читайте далее: [10_Loading_Flow.md](10_Loading_Flow.md)*
