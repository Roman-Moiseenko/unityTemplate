# 12. Сервисы Gameplay

## Полный список сервисов

| № | Сервис | Основные обязанности | Ключевые зависимости |
|---|--------|---------------------|---------------------|
| 1 | `WayService` | Расчёт пути для мобов: поиск точек входа/выхода, построение маршрута | Нет (чистая математика) |
| 2 | `PlacementService` | Проверка размещения объектов: можно ли поставить башню/дорогу, направление | `GameplayStateProxy`, `WayService` |
| 3 | `RoadsService` | Управление дорогами: создание, удаление | `GameplayStateProxy`, `ICommandProcessor` |
| 4 | `GroundsService` | Управление землёй (клетками): создание, удаление | `GameplayStateProxy`, `ICommandProcessor` |
| 5 | `WarriorService` | Управление воинами: создание, удаление, FsmWarrior | `GameplayStateProxy`, `ICommandProcessor` |
| 6 | `TowersService` | Управление башнями: создание, удаление, перемещение, улучшение, замена, бустеры, выстрелы | `GameplayStateProxy`, `TowersSettings`, `PlacementService`, `FsmTower`, `FsmWave` |
| 7 | `SkillsService` | Управление скиллами | `GameplayStateProxy`, `SkillsSettings` |
| 8 | `HeroService` | Управление героем (бустеры, баффы) | `GameplayStateProxy`, `GameplayEnterParams` |
| 9 | `FrameSkillService` | Визуальный фрейм для скиллов | `GameplayStateProxy`, `PlacementService`, `SkillsService`, `RoadsService`, `SkillsSettings`, `IQueryProcessor` |
| 10 | `FrameService` | Визуальный фрейм выделения башен | `GameplayStateProxy`, `PlacementService`, `TowersService`, `RoadsService`, `TowersSettings`, `IQueryProcessor` |
| 11 | `FramePlacementService` | Фрейм размещения новой башни | `GameplayStateProxy`, `PlacementService`, `FsmTower`, `TowersService` |
| 12 | `CastleService` | Управление жизнью замка, воскрешение | `GameplayStateProxy` |
| 13 | `RewardProgressService` | Прогресс и награды за убийства | `GameplayStateProxy`, `GameSettings`, `GameplayEnterParams` |
| 14 | `GameplayService` | Отслеживание конца игры: победа, поражение, выход | `Subject<GameplayExitParams>`, `GameplayStateProxy`, `AdService`, `ResourceService`, `ICommandProcessor` |
| 15 | `DamageService` | Расчёт урона по мобам, дебаффы, попапы урона | `GameplayStateProxy`, `GameSettingsStateProxy` |
| 16 | `WaveService` | Управление волнами мобов: таймер, генерация, GateWave, FsmWave | `GameplayStateProxy`, `ICommandProcessor`, `WayService`, `FsmWave`, `RoadsService` |
| 17 | `GameplayCamera` | Управление камерой (приближение/отдаление) | `DIContainer` |
| 18 | `InputController` | Обработка ввода | `DIContainer` |

## Детальное описание ключевых сервисов

### WaveService

Управляет полным жизненным циклом волн:

```csharp
public class WaveService : IDisposable
{
    // Таймер между волнами (0..1)
    public ReactiveProperty<float> TimeOutNewWaveValue;
    
    // Запуск
    public void Start();                              // Первый запуск таймера
    public void StartNextWave();                      // Принудительный запуск
    public void StartForcedNewWave();                 // Мгновенный старт (кнопка)
    
    // Построение пути для моба
    public List<RoadPoint> GenerateRoadPoints(MobEntity mobEntity);
}
```

Цикл работы:
1. **FsmStateWaveTimer** — обратный отсчёт через корутину с `WaitForSeconds`
2. **FsmStateWaveBegin** — увеличивает `CurrentWave`, создаёт мобов в буфер
3. **FsmStateWaveGo** — асинхронный вывод мобов из буфера с паузой `SPEED_GENERATE_MOBS`
4. **FsmStateWaveWait** — ждёт, пока все мобы не будут убиты
5. **GateWave** — создаёт и двигает ворота по мере удлинения дороги
6. **Debuff** — таймеры на дебаффы мобов (замедление)

### TowersService

Управляет всеми башнями на карте:

```csharp
public class TowersService : IDisposable
{
    // Основные операции
    public void PlaceTower(string towerTypeId, Vector2Int position);
    public void DeleteTower(int towerId);
    public void MoveTower(int towerId, Vector2Int position);
    public void LevelUpTower(string configId);
    public void ReplaceTower(int cardUniqueId, int cardUniqueId2);
    
    // Расчёт
    public void CalculateBoosters();                              // Бустеры от героя и защиты
    public ShotData ShotCalculation(TowerEntity, TypeDefence);    // Расчёт выстрела
}
```

Кеширует:
- `TowerParametersMap` — параметры башен по ConfigId
- `Levels` — уровни башен по ConfigId
- `TowerBoosters` — бустеры по ConfigId (урон, крит, скорость, дистанция)

### PlacementService

Проверка размещения объектов на карте:

```csharp
public class PlacementService : IDisposable
{
    // Проверка башни
    public bool CheckPlacementTower(Vector2Int position, int towerId, bool onRoad, bool isPlacement);
    
    // Проверка дороги
    public bool CheckPlacementRoad(List<RoadEntity> roads);
    
    // Направления
    public RoadDirection GetDirectionTower(Vector2Int position);       // Направление от башни к дороге
    public Vector2Int GetDefaultPlacement(Vector2Int position);       // Ближайшая дорога
    public RoadDirection GetRoadDirectionNext(Vector2Int position);    // Направление дороги (следующая точка)
    public RoadDirection GetRoadDirectionPrevious(Vector2Int position);// Направление дороги (предыдущая точка)
    
    // Проверка
    public bool IsRoad(Vector2Int position);
}
```

Проверка башни включает:
- Не на замке
- На земле (не пустота)
- Не на другой башне
- Рядом с дорогой (если не `isPlacement`)
- Свободное место для плейсмента

### DamageService

Нанесение урона:

```csharp
public class DamageService : IDisposable
{
    // Подписывается на gameplayState.Shots.ObserveAdd()
    // При появлении выстрела — находит моба, применяет урон
    // Создаёт DamageEntity для отображения попапа урона
    // Применяет дебаффы к мобу
}
```

### GameplayService

Отслеживание конца игры и наград:

```csharp
public class GameplayService : IDisposable
{
    public void Win();         // Все волны пройдены → расчёт наград
    public void Lose();        // Замок разрушен (2 смерти)
    public void Abort();       // Прерывание (выход в меню)
    public void ExitSave();    // Выход с сохранением сессии
    public void RepairCristal(); // Воскрешение замка за кристаллы
    public void RepairAd();    // Воскрешение замка за рекламу
}
```

Расчёт наград:
- `GetRewardOnWave(completedLevel)` — награды за волны из `MapRewardSetting.RewardOnWave`
- `GetTypeChestWin()` — тип сундука при победе
- `GetTypeChestLose(lastWave)` — тип сундука при поражении (зависит от пройденных волн)

## Особенности сервисов

- Все сервисы реализуют `IDisposable`
- Регистрируются через `container.RegisterDisposableOnSceneExit()` для автоочистки
- Используют CQRS для изменения состояния
- Подписываются на реактивные коллекции `GameplayStateProxy`
- Порядок регистрации важен из-за зависимостей между сервисами

---

*Читайте далее: [13_UIManager.md](13_UIManager.md)*
