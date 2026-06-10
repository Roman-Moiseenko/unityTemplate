# 11. Gameplay — основная архитектура сцены

## GameplayEntryPoint

`GameplayEntryPoint` — MonoBehaviour на сцене, который запускается из `GameEntryPoint` после загрузки сцены.

```csharp
public class GameplayEntryPoint : MonoBehaviour
{
    [SerializeField] private UIGameplayRootBinder _sceneUIRootPrefab;  // Префаб UI
    [SerializeField] private WorldGameplayRootBinder _worldRootBinder; // Мировой корень
    
    public Observable<GameplayExitParams> Run(DIContainer gameplayContainer, GameplayEnterParams enterParams)
    {
        // 1. Регистрация событий (Subject для выхода, камера)
        // 2. GameplayRegistrations.Register() — сервисы, FSM, CQRS
        // 3. GameplayViewModelsRegistrations.Register() — ViewModel
        // 4. InitWorld() — построение мира
        // 5. InitUI() — присоединение UI
        // 6. Сохранение начального состояния
        // 7. WaveService().Start() — запуск волн
        // 8. Возврат Observable<GameplayExitParams>
    }
    
    private void OnDestroy()
    {
        // Очистка: DisposeSceneDisposables(), DisposeGameplay()
    }
}
```

## Входные и выходные параметры

### GameplayEnterParams

```csharp
public class GameplayEnterParams : SceneEnterParams
{
    public int MapId;                                // ID карты
    public float GameSpeed;                          // Скорость игры
    public TypeGameplay TypeGameplay;                // Тип игры (Levels/Infinity/Event/Resume)
    public List<TowerCardData> Towers;               // Колода башен
    public List<SkillCardData> Skills;               // Колода скиллов
    public HeroCardData HeroCard;                    // Карточка героя
    public GameplayBoosters GameplayBoosters;        // Бустеры (урон, крит, скорость)
}
```

### GameplayExitParams

```csharp
public class GameplayExitParams
{
    public MainMenuEnterParams MainMenuEnterParams;  // Награды и результаты
    public bool SaveGameplay;                        // Сохранить сессию
}
```

### MainMenuEnterParams (результаты сессии)

```csharp
public class MainMenuEnterParams
{
    public string Result;                    // Сообщение
    public long SoftCurrency;                // Заработанное золото
    public int LastWave;                     // Последняя пройденная волна
    public bool FinishedMap;                 // Уровень пройден
    public float GameSpeed;                  // Скорость игры
    public int MapId;                        // ID карты
    public int KillsMob;                     // Количество убитых мобов
    public TypeGameplay TypeGameplay;        // Тип игры
    public int LastRewardOnWave;             // Последняя награда за волну
    public TypeChest LastRewardChest;        // Тип последнего сундука
    public TypeChest TypeChest;              // Тип нового сундука
    public List<InventoryItemData> Items;
    public List<RewardEntityData> RewardCards;     // Карты-награды
    public List<RewardEntityData> RewardOnWave;    // Награды за волны
}
```

## Flow запуска сцены Gameplay

```
GameEntryPoint (корень)
    ↓ LoadScene(GAMEPLAY) + LoadGameplayState()
    ↓
GameplayEntryPoint.Run(gameplayContainer, enterParams)
    ↓
GameplayRegistrations.Register(container, enterParams)
    │
    ├── 1. Загрузка GameStateProvider, GameSettings
    ├── 2. Определение defaultGroundConfigId / defaultRoadConfigId
    │      (зависит от TypeGameplay)
    │
    ├── 3. Регистрация FSM:
    │      ├── FsmGameplay (GamePause, GamePlay, BuildBegin, Build, BuildEnd, SelectSkill, SetSkill)
    │      ├── FsmWave (Timer, Begin, Go, End, Wait)
    │      ├── FsmTower (None, Selected, Delete, Placement, PlacementEnd)
    │      └── FsmSkill (None, Begin, SetTarget, ShowEffect, End)
    │
    ├── 4. Регистрация QueryProcessor + хендлеры:
    │      ├── QueryInfoWave
    │      ├── QueryInfoTower
    │      └── QueryInfoSkill
    │
    ├── 5. Регистрация CommandProcessor + все CommandHandler'ы:
    │      (Ground, Tower, Wave, Mob, Castle, Road, Level, Infinity, Reward, Delete, Move, Replace)
    │
    ├── 6. Создание и регистрация сервисов в строгом порядке зависимостей:
    │      WayService → PlacementService → RoadsService → GroundsService → WarriorService →
    │      TowersService → SkillsService → HeroService → FrameSkillService → FrameService →
    │      FramePlacementService → CastleService → RewardProgressService → GameplayService →
    │      DamageService → WaveService
    │
    ├── 7. Загрузка карты (CommandCreateLevel / CommandCreateInfinity)
    │      Если Towers пуст — создаются начальные башни из настроек карты
    │
    └── 8. FsmGameplay → FsmStateBuildBegin (начало в режиме строительства)
    ↓
GameplayViewModelsRegistrations.Register(viewContainer)
    │
    ├── Регистрация Subject для UI (CLICK_WORLD_ENTITY)
    ├── Регистрация Subject для клика по башне
    ├── UIGameplayRootViewModel (управление UI)
    ├── GameplayUIManager (менеджер экранов/попапов/панелей)
    └── WorldGameplayRootViewModel (все сервисы для отображения мира)
    ↓
InitWorld() и InitUI()
    │
    ├── WorldGameplayRootBinder.Bind(WorldGameplayRootViewModel)
    └── UI: создание UIRoot, прикрепление префаба, UIGameplayRootBinder.Bind()
    ↓
WaveService.Start() → FsmWave → FsmStateWaveTimer → запуск таймера первой волны
```

## Компоненты сцены

| Компонент | Тип | Описание |
|-----------|-----|----------|
| `GameplayEntryPoint` | MonoBehaviour | Точка входа сцены, управляет жизненным циклом |
| `UIGameplayRootBinder` | MonoBehaviour (префаб) | UI корень (Canvas, панели, попапы) |
| `WorldGameplayRootBinder` | MonoBehaviour (на сцене) | Мировой корень (игровое поле, камера) |

## Инициализация мира (InitWorld)

При загрузке карты:
1. Создаются клетки земли (`GroundEntity`) вокруг дороги
2. Создаются дороги (`RoadEntity`) по начальной конфигурации карты
3. Создаётся замок (`CastleEntity`) в заданной позиции
4. Создаются начальные башни (если указаны в `MapData.InitialTowers`)
5. Расставляются ворота (`GateWave`) у входа на дорогу

## Инициализация UI (InitUI)

1. Создаётся `UIRoot` из префаба
2. К нему прикрепляется `UIGameplayRootBinder`
3. ViewModel связывается с Binder
4. Отображается `ScreenGameplay`
5. Отображаются начальные панели (например, `PanelGateWave`)

---

*Читайте далее: [12_Gameplay_Services.md](12_Gameplay_Services.md)*
