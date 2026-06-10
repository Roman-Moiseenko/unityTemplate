# 7. Game State (Состояние игры) — трёхуровневая система

## Три уровня состояния

| Уровень | POCO | Proxy | Назначение |
|---------|------|-------|------------|
| **GameState** | `GameState` | `GameStateProxy` | Данные игрока (валюта, инвентарь, прогресс по картам, сундуки) |
| **GameplayState** | `GameplayState` | `GameplayStateProxy` | Состояние текущей сессии (башни, мобы, волны, дороги, замок) |
| **GameSettingsState** | `GameSettingsState` | `GameSettingsStateProxy` | Личные настройки пользователя (звук, вибрация, токены) |

## GameState

### Структура GameState (POCO)

```csharp
public class GameState
{
    public int GlobalInventoryId;              // Глобальный ID для инвентаря
    public int CurrentMapId;                   // Текущая карта
    public float GameSpeed;                    // Скорость игры
    public InventoryRootData Inventory;        // Инвентарь (карты башен, скиллов, планы)
    public MapStatesData MapStatesData;        // Прогресс по картам (результаты, награды)
    public long HardCurrency;                  // Кристаллы (премиум-валюта)
    public long SoftCurrency;                  // Золото (обычная валюта)
    public ContainerChestsData ContainerChests;// Сундуки
    public DateTime DateVersion;               // Версия данных для синхронизации
}
```

### GameStateProxy (реактивная обёртка)

```csharp
public class GameStateProxy : IDisposable
{
    public readonly GameState Origin;                    // Оригинальные данные
    
    // Реактивные свойства (изменение → автосинхронизация с Origin)
    public ReactiveProperty<long> HardCurrency;
    public ReactiveProperty<long> SoftCurrency;
    public ReactiveProperty<int> CurrentMapId;
    public ReactiveProperty<float> GameSpeed;
    
    public InventoryRoot Inventory;                      // Реактивная обёртка инвентаря
    public MapStates MapStates;                          // Прогресс по картам
    public ContainerChests ContainerChests;              // Сундуки
}
```

## GameplayState

### Структура GameplayState (POCO)

```csharp
public class GameplayState
{
    public int MapId;                              // ID карты
    public int CurrentWave;                        // Текущая волна
    public int GlobalEntityId;                     // Автоинкрементный ID для сущностей
    public float GameSpeed;                        // Скорость игры
    public int Progress;                           // Прогресс (растёт от убийств мобов)
    public int ProgressLevel;                      // Уровень прогресса
    public long SoftCurrency;                      // Золото в сессии
    public int UpdateCards;                        // Количество улучшений карточек
    public TypeGameplay TypeGameplay;              // Тип игры (Levels, Infinity, Event, Resume)
    public float TotalTimeInScene;                 // Общее время на сцене
    public CastleEntityData CastleData;            // Данные замка
    public HeroEntityData HeroData;                // Данные героя
    public StatisticGameData StatisticGameData;    // Статистика (убийства, урон)
    public bool HasWaySecond;                      // Есть ли второй путь
    
    // Списки сущностей
    public List<RewardEntityData> RewardEntities;
    public List<TowerEntityData> Towers;
    public List<SkillEntityData> Skills;
    public List<GroundEntityData> Grounds;
    public List<WarriorEntityData> Warriors;
    public List<RoadEntityData> Way;
    public List<RoadEntityData> WaySecond;
    public List<RoadEntityData> WayDisabled;
    
    public SkillEntityData SkillOne;               // Первый скилл
    public SkillEntityData SkillTwo;               // Второй скилл
}
```

### GameplayStateProxy (реактивная обёртка)

```csharp
public class GameplayStateProxy : IDisposable
{
    public readonly GameplayState Origin;
    
    // ObservableCollections для отслеживания добавления/удаления
    public ObservableList<TowerEntity> Towers;
    public ObservableList<MobEntity> Mobs;
    public ObservableList<MobEntity> BufferMobs;
    public ObservableList<MobEntity> SecondBufferMobs;
    public ObservableList<RoadEntity> Way;
    public ObservableList<RoadEntity> WaySecond;
    public ObservableList<RoadEntity> WayDisabled;
    public ObservableList<GroundEntity> Grounds;
    public ObservableList<WarriorEntity> Warriors;
    public ObservableList<SkillEntity> Skills;
    public ObservableList<ShotData> Shots;
    
    // ReactiveProperty для ключевых полей
    public ReactiveProperty<int> Progress;
    public ReactiveProperty<int> ProgressLevel;
    public ReactiveProperty<long> SoftCurrency;
    public ReactiveProperty<int> MapId;
    public ReactiveProperty<bool> HasWaySecond;
    public ReactiveProperty<int> CurrentWave;
    public ReactiveProperty<int> UpdateCards;
    public ReactiveProperty<float> TotalTimeInScene;
    public ReactiveProperty<TypeGameplay> TypeGameplay;
    public ReactiveProperty<SkillEntity> SkillOne;
    public ReactiveProperty<SkillEntity> SkillTwo;
    public ReactiveProperty<bool> MapFinished;
    
    public CastleEntity Castle;
    public HeroEntity Hero;
    public StatisticGame StatisticGame;
    public ReadOnlyReactiveProperty<Vector2> GateWave;
    public ReadOnlyReactiveProperty<Vector2> GateWaveSecond;
    
    public int CountWaves;                        // Всего волн (из настроек)
    public GameplayEnterParams EnterParams;        // Входные параметры
    
    public int CreateEntityID() => Origin.GlobalEntityId++;
}
```

## GameSettingsState

```csharp
public class GameSettingsState
{
    public string UserId;           // ID пользователя
    public string UserToken;        // Токен авторизации
    public bool Vibration;          // Вибрация
    public bool Damage;             // Показывать урон
    public bool Sound;              // Звук
    public bool Music;              // Музыка
    public int MusicVolume;         // Громкость музыки
    public int SFXVolume;           // Громкость звуков
}

public class GameSettingsStateProxy : IDisposable
{
    public readonly GameSettingsState Origin;
    
    public ReactiveProperty<string> UserId;
    public ReactiveProperty<string> UserToken;
    public ReactiveProperty<bool> Vibration;
    public ReactiveProperty<bool> Damage;
    public ReactiveProperty<bool> Sound;
    public ReactiveProperty<bool> Music;
    public ReactiveProperty<int> MusicVolume;
    public ReactiveProperty<int> SFXVolume;
}
```

## Паттерн Proxy

### Принцип синхронизации

```
EntityData (POCO, сериализуемый JSON)
    ↓
Entity (TowerEntity, MobEntity, etc.) — реактивная обёртка
    ↓
ObservableList<Entity> — отслеживание добавления/удаления
    ↓
GameplayStateProxy — содержит ObservableList'ы и ReactiveProperty
```

Принцип:
- `EntityData` → `Entity`: поля EntityData оборачиваются в `ReactiveProperty`
- Изменение `ReactiveProperty` → обновление поля в `EntityData`
- Добавление в `ObservableList` → добавление в список `EntityData`
- Удаление — удаление из обоих списков

## IGameStateProvider

```csharp
public interface IGameStateProvider : IDisposable
{
    GameStateProxy GameState { get; }
    GameSettingsStateProxy SettingsState { get; }
    GameplayStateProxy GameplayState { get; }
    
    // Загрузка
    Observable<LoadingState> LoadGameState();
    Observable<LoadingState> LoadSettingsState();
    Observable<GameplayStateProxy> LoadGameplayState();
    
    // Сохранение
    Observable<bool> SaveGameState();
    Observable<bool> SaveGameStateWeb();
    Observable<bool> SaveGameStateLocal();
    Observable<bool> SaveSettingsState();
    Observable<bool> SaveGameplayState();
    Observable<bool> ResetGameplayState();
    
    void DisposeGameplay();
}
```

### Реализация: WebGameStateProvider

Загружает/сохраняет одновременно через веб (UnityWebRequest) и локально (PlayerPrefs). При загрузке сравнивает `DateVersion` и выбирает актуальную версию.

**Процесс загрузки:**
1. Загрузка локального состояния из PlayerPrefs
2. Если есть веб-доступ — загрузка с сервера
3. Сравнение DateVersion — выбор актуального
4. При сохранении — дуальная запись (локально + сервер)

---

*Читайте далее: [08_Entity_System.md](08_Entity_System.md)*
