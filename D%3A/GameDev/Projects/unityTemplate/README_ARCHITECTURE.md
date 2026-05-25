# Архитектура Unity-проекта (Tower Defense)

## Оглавление

1. [Общий обзор архитектуры](#общий-обзор-архитектуры)
2. [DI-контейнер (Inversion of Control)](#di-контейнер-inversion-of-control)
3. [CQRS — Command & Query Pattern](#cqrs--command--query-pattern)
4. [FSM — Finite State Machine](#fsm--finite-state-machine)
5. [MVVM — Model-View-ViewModel](#mvvm--model-view-viewmodel)
6. [Game State (Состояние игры)](#game-state-состояние-игры)
7. [Gameplay — Основная архитектура сцены](#gameplay--основная-архитектура-сцены)
8. [Сервисы Gameplay](#сервисы-gameplay)
9. [Settings Provider](#settings-provider)
10. [Поток загрузки игры](#поток-загрузки-игры)
11. [Работа с UI (UIManager)](#работа-с-ui-uimanager)
12. [Заключение и принципы](#заключение-и-принципы)

---

## Общий обзор архитектуры

Проект представляет собой **Tower Defense** игру на Unity с использованием следующих ключевых архитектурных паттернов:

- **DI (Dependency Injection)** — самописный контейнер для управления зависимостями
- **CQRS** (Command Query Responsibility Segregation) — разделение команд (изменение состояния) и запросов (чтение данных)
- **FSM (Finite State Machine)** — конечные автоматы для управления состояниями игры
- **MVVM (Model-View-ViewModel)** — паттерн для связывания данных с UI через реактивные свойства
- **Proxy и Reactive Extensions** (через библиотеку `R3` и `ObservableCollections`) — реактивное отслеживание изменений

### Основные директории

```
Assets/Scripts/
├── DI/                        # Dependency Injection контейнер
│   ├── DIContainer.cs         # Основной контейнер
│   └── DIEntry.cs             # Entry для фабрик и singleton
├── Game/
│   ├── GamePlay/              # Всё, что относится к геймплею
│   │   ├── Root/              # Точка входа в сцену Gameplay
│   │   ├── Fsm/               # Конечные автоматы
│   │   ├── Services/          # Игровые сервисы
│   │   ├── Commands/          # CQRS команды (обработчики)
│   │   ├── Queries/           # CQRS запросы (обработчики)
│   │   ├── Classes/           # Вспомогательные классы
│   │   └── View/              # MVVM (ViewModels + View)
│   │       ├── Mobs/          # ViewModel мобов
│   │       ├── Towers/        # ViewModel башен
│   │       ├── UI/            # UI-элементы (панели, попапы)
│   │       └── ...
│   ├── GameRoot/              # Точка входа в игру
│   │   ├── GameEntryPoint.cs  # Старт игры
│   │   ├── Scenes.cs          # Список сцен
│   │   └── Services/          # Глобальные сервисы (Ad, Resource)
│   ├── MainMenu/              # Главное меню
│   ├── Settings/              # Настройки игры
│   ├── State/                 # Состояние игры (GameState)
│   │   ├── Root/              # Proxy для GameState
│   │   ├── Gameplay/          # Proxy для сессии геймплея
│   │   ├── Maps/              # Сущности карты (Mob, Tower, ...)
│   │   └── ...
│   └── Common/                # Общие константы и утилиты
├── MVVM/                      # MVVM фреймворк
│   ├── UI/                    # Базовые UI-компоненты
│   ├── CMD/                   # CQRS (ICommand, IQuery)
│   ├── FSM/                   # Конечные автоматы
│   └── Storage/               # Object Pooling
├── Utils/                     # Вспомогательные утилиты
└── Localization/              # Локализация
```

---

## DI-контейнер (Inversion of Control)

`DI.DIContainer` — самописный IoC-контейнер, основанный на концепции вложенных контейнеров (parent scoping).

### Ключевые возможности

```csharp
// Создание контейнера (можно указать родительский)
var container = new DIContainer(parentContainer);

// Регистрация фабрики (создаётся при каждом Resolve)
container.RegisterFactory<IService>(c => new SomeService());

// Регистрация singleton (создаётся один раз при первом Resolve)
container.RegisterFactory<IService>(c => new SomeService()).AsSingle();

// Регистрация с тегом
container.RegisterFactory<IService>("special", c => new SpecialService());

// Регистрация готового экземпляра
container.RegisterInstance<IService>(existingInstance);

// Разрешение зависимости
var service = container.Resolve<IService>();
var tagged = container.Resolve<IService>("special");
```

### Управление жизненным циклом

```csharp
// Регистрация disposable-объектов, которые будут уничтожены при выходе со сцены
container.RegisterDisposableOnSceneExit(myDisposable);

// При уничтожении сцены все такие объекты дизпозятся в обратном порядке
container.DisposeSceneDisposables();
```

### Вложенные контейнеры

Контейнер строит иерархию: `RootContainer` → `SceneContainer` (MainMenu, Gameplay). При разрешении зависимости, если она не найдена в текущем контейнере, поиск идёт в родительском.

---

## CQRS — Command & Query Pattern

Архитектура разделяет модификацию состояния (команды) и чтение (запросы).

### Команды (Commands)

```csharp
// Маркерный интерфейс
public interface ICommand { }

// Обработчик команды — возвращает bool (успех/неудача)
public interface ICommandHandler<TCommand> where TCommand : ICommand
{
    bool Handle(TCommand command);
}

// Процессор команд
public interface ICommandProcessor
{
    void RegisterHandler<TCommand>(ICommandHandler<TCommand> handler);
    bool Process<TCommand>(TCommand command, bool autoSave = true);
}
```

**Пример команды:**

```csharp
// Команда (просто DTO с данными)
public class CommandPlaceTower : ICommand
{
    public readonly string ConfigId;
    public readonly Vector2Int Position;
    public Vector2Int Placement { get; set; }
}

// Обработчик — содержит бизнес-логику
public class CommandPlaceTowerHandler : ICommandHandler<CommandPlaceTower>
{
    public bool Handle(CommandPlaceTower command)
    {
        var entityId = _gameplayState.CreateEntityID();
        // ... создание TowerEntity, добавление в состояние
        _gameplayState.Towers.Add(newTower);
        return true;
    }
}
```

Существует **два типа CommandProcessor**:

| Процессор | Сохраняет | Использование |
|-----------|-----------|---------------|
| `CommandProcessor` | `SaveGameState()` + `SaveSettingsState()` | Глобальные команды (валюта, инвентарь) |
| `CommandProcessorGameplay` | `SaveGameplayState()` | Игровые команды (построить башню, создать моба) |

### Запросы (Queries)

```csharp
// Маркерный интерфейс
public interface IQuery { }

// Обработчик запроса — принимает ISettingsProvider для доступа к настройкам
public interface IQueryHandler<TQuery> where TQuery : IQuery
{
    object Handle(TQuery query, ISettingsProvider settingsProvider);
}

// Процессор запросов
public interface IQueryProcessor
{
    void RegisterHandler<TQuery>(IQueryHandler<TQuery> handler);
    object Request<TQuery>(TQuery query);
}
```

### Принцип работы

1. **Регистрация** — все обработчики регистрируются в процессоре при загрузке сцены
2. **Выполнение** — вызов `Process()` или `Request()` передаёт команду/запрос нужному обработчику
3. **Автосохранение** — после успешной обработки команды состояние автоматически сохраняется

---

## FSM — Finite State Machine

### Базовая реализация (`MVVM.FSM.FSM`)

```csharp
public class FSM
{
    public FSMState StateCurrent { get; private set; }
    public FSMState PreviousState { get; private set; }
    public Dictionary<Type, FSMState> States = new();

    public void AddState(FSMState state);
    public void SetState<T>() where T : FSMState;
    public void Update();
}

public abstract class FSMState
{
    public abstract void Enter();
    public abstract bool Exit(FSMState next = null);
    public abstract void Update();
}
```

### FsmProxy — реактивная обёртка

`FsmProxy` — это обёртка, которая предоставляет `ReactiveProperty<FSMState>` для подписки на смену состояний.

### Автоматы в проекте

В проекте используется **пять конечных автоматов**:

| Автомат | Состояния | Назначение |
|---------|-----------|------------|
| `FsmGameplay` | `GamePause`, `GamePlay`, `SelectSkill`, `SetSkill`, `BuildBegin`, `Build`, `BuildEnd` | Общее состояние геймплея (пауза/строительство/игра) |
| `FsmTower` | `None`, `Selected`, `Delete`, `Placement`, `PlacementEnd` | Состояние взаимодействия с башней |
| `FsmWave` | `Begin`, `Go`, `End`, `Wait`, `Timer` | Состояние волны мобов |
| `FsmSkill` | `None`, `Begin`, `SetTarget`, `ShowEffect`, `End` | Состояние применения скилла |
| `FsmWarrior` | `New`, `Await`, `GoToMob`, `Attack`, `GoToRepair`, `Repair`, `GoToPlacement`, `Dead` | Состояние воина |

### Пример использования

```csharp
// Создание автомата
var fsmGameplay = new FsmGameplay(container);

// Установка состояния
fsmGameplay.Fsm.SetState<FsmStateBuildBegin>();

// Подписка на смену состояния
fsmGameplay.Fsm.StateCurrent.Subscribe(newState => {
    if (newState is FsmStateGamePlay) { /* ... */ }
});

// Проверка текущего состояния
bool isPlaying = fsmGameplay.IsStateGamePlay();
bool isBuilding = fsmGameplay.IsStateBuilding();
```

---

## MVVM — Model-View-ViewModel

### Архитектура MVVM в проекте

- **Model** — данные (`EntityData`, `GameState`)
- **ViewModel** — реактивная обёртка, преобразующая модель в данные для UI
- **View** — Binder (MonoBehaviour), подписывающийся на ViewModel и обновляющий Unity-компоненты

Все реакции реализованы через **R3** (`ReactiveProperty`, `Subject`, `Observable`) и **ObservableCollections**.

### Базовые UI-компоненты

```
MVVM/UI/
├── UIRootViewModel.cs     # Корневая ViewModel (содержит Screen, Popups, Panels)
├── UIRootBinder.cs        # Биндер корневого UI
├── PanelBinder.cs         # Базовый класс для панелей
├── WindowBinder.cs        # Базовый класс для окон
├── PopupBinder.cs         # Базовый класс для попапов
├── DropdownBinder.cs      # Базовый класс для выпадающих списков
└── UIManager.cs           # Менеджер UI (открытие/закрытие)
```

### Пример связки

**ViewModel:**
```csharp
public class PanelActionsViewModel : PanelViewModel
{
    public ReactiveProperty<bool> IsVisible = new(false);
    public Subject<Unit> OnBuildClicked = new();
    
    public PanelActionsViewModel(UIManager uiManager, DIContainer container) 
        : base(uiManager, container)
    {
        // подписка на события
    }
}
```

**Binder:**
```csharp
public class PanelActionsBinder : PanelBinder
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private Button _buildButton;

    private void Bind(PanelActionsViewModel viewModel)
    {
        viewModel.IsVisible.Subscribe(v => _panel.SetActive(v));
        _buildButton.onClick.AddListener(() => viewModel.OnBuildClicked.OnNext(Unit.Default));
    }
}
```

---

## Game State (Состояние игры)

### Три уровня состояния

| Уровень | Класс | Назначение |
|---------|-------|------------|
| **GameState** | `GameState` + `GameStateProxy` | Данные игрока (валюта, инвентарь, прогресс по картам) |
| **GameplayState** | `GameplayState` + `GameplayStateProxy` | Состояние текущей сессии (построенные башни, мобы, волны) |
| **GameSettingsState** | `GameSettingsState` + `GameSettingsStateProxy` | Личные настройки пользователя |

### Паттерн Proxy

Для каждого уровня состояния создаётся **Proxy** — реактивная обёртка:

```
GameState (POCO, сериализуемый)
    ↓
GameStateProxy (реактивная обёртка)
    ↓ Подписка на изменения → автосинхронизация с оригиналом
    ↓ ObservableCollection, ReactiveProperty
```

**GameStateProxy:**
```csharp
public class GameStateProxy : IDisposable
{
    public readonly GameState Origin;       // Оригинальные данные
    
    // Реактивные свойства
    public ReactiveProperty<long> HardCurrency;
    public ReactiveProperty<long> SoftCurrency;
    public ReactiveProperty<int> CurrentMapId;
    public InventoryRoot Inventory;
    public MapStates MapStates;
    
    // Любое изменение реактивного свойства → обновление Origin
    public GameStateProxy(GameState gameState) { /* подписки */ }
}
```

**GameplayStateProxy:**
```csharp
public class GameplayStateProxy : IDisposable
{
    public readonly GameplayState Origin;
    
    // ObservableCollections для отслеживания добавления/удаления
    public ObservableList<TowerEntity> Towers { get; }
    public ObservableList<MobEntity> Mobs { get; }
    public ObservableList<RoadEntity> Way { get; }
    public ObservableList<GroundEntity> Grounds { get; }
    // ...
    
    // При добавлении в ObservableList → автосинхронизация с Origin
}
```

### Сохранение и загрузка

```csharp
public interface IGameStateProvider : IDisposable
{
    GameStateProxy GameState { get; }           // Данные игрока
    GameSettingsStateProxy SettingsState { get; } // Настройки
    GameplayStateProxy GameplayState { get; }    // Текущая сессия
    
    Observable<LoadingState> LoadGameState();
    Observable<bool> SaveGameState();
    
    Observable<LoadingState> LoadSettingsState();
    Observable<bool> SaveSettingsState();
    
    Observable<GameplayStateProxy> LoadGameplayState();
    Observable<bool> SaveGameplayState();
    Observable<bool> ResetGameplayState();
}
```

Реализации: `WebGameStateProvider` (для прода) и `PlayerPrefsGameStateProvider` (для тестов).

---

## Gameplay — Основная архитектура сцены

### Flow запуска сцены Gameplay

```
GameEntryPoint (корень)
    ↓ LoadScene(GAMEPLAY)
    ↓ LoadGameplayState()
    ↓
GameplayEntryPoint (MonoBehaviour на сцене)
    ↓ Run(gameplayContainer, enterParams)
    ↓
GameplayRegistrations.Register(container, enterParams)
    ├── Регистрация всех FSM
    ├── Регистрация CQRS (CommandProcessor, QueryProcessor, хендлеры)
    ├── Создание и регистрация сервисов
    └── Загрузка карты (CommandCreateLevel / CommandCreateInfinity)
    ↓
GameplayViewModelsRegistrations.Register(viewContainer)
    └── Регистрация UIGameplayRootViewModel, WorldGameplayRootViewModel, UIManager
    ↓
InitWorld() и InitUI()
    ↓
WaveService.Start() → запуск таймера первой волны
```

### Компоненты сцены

| Компонент | Описание |
|-----------|----------|
| `GameplayEntryPoint` | Точка входа сцены |
| `UIGameplayRootBinder` | UI корень (загружается из префаба) |
| `WorldGameplayRootBinder` | Мировой корень (игровые объекты) |

---

## Сервисы Gameplay

### Основные сервисы

| Сервис | Описание |
|--------|----------|
| `WaveService` | Управление волнами мобов (таймер, генерация, запуск) |
| `TowersService` | Управление башнями (создание, удаление, перемещение, улучшение) |
| `SkillsService` | Управление скиллами |
| `CastleService` | Управление здоровьем замка |
| `WayService` | Расчёт пути для мобов |
| `RoadsService` | Управление дорогами (строительство/удаление) |
| `GroundsService` | Управление землёй (клетками) |
| `PlacementService` | Расчёт позиционирования объектов на карте |
| `FrameService` | Визуальный фрейм выделения для башен |
| `FrameSkillService` | Визуальный фрейм выделения для скиллов |
| `FramePlacementService` | Визуальный фрейм размещения башни |
| `WarriorService` | Управление воинами |
| `DamageService` | Расчёт урона |
| `RewardProgressService` | Расчёт прогресса и наград |
| `GameplayService` | Отслеживание конца игры (победа/поражение/выход) |
| `InputController` | Обработка ввода |

### Особенности сервисов

- Все сервисы реализуют `IDisposable`
- Регистрируются через `container.RegisterDisposableOnSceneExit()` для автоматической очистки
- Используют CQRS для изменения состояния
- Подписываются на реактивные коллекции `GameplayStateProxy`

---

## Settings Provider

```csharp
public interface ISettingsProvider
{
    GameSettings GameSettings { get; }
    Observable<LoadingState> LoadGameSettings();
}
```

`GameSettings` содержит:

- `MapsSettings` — настройки всех карт (волны, награды, расположения)
- `TowersSettings` — настройки всех башен (уровни, параметры)
- `SkillsSettings` — настройки всех скиллов
- `EnemiesSettings` — настройки врагов
- `GroundSettings` — настройки земли

Реализация: `SettingsProviderWeb` (загрузка с сервера).

---

## Поток загрузки игры

```
GameEntryPoint.AutostartGame()
    ↓
1. Создание корневого контейнера
2. Регистрация UIRoot, StorageManager, Settings, CQRS, глобальных сервисов
    ↓
LoadFirstBoot()
    ↓
1. Показать экран загрузки
2. Загрузить Boot сцену → MainMenu сцену
3. Проверка веб-доступа
4. Загрузка настроек (LoadSettingsState)
5. Загрузка данных игрока (LoadGameState)
6. Загрузка настроек игры (LoadGameSettings)
7. Запуск MainMenuEntryPoint.Run()
    ↓
MainMenu → выбор карты → GameplayEnterParams
    ↓
LoadAndStartGameplay(enterParams)
    ↓
1. Загрузить Boot сцену → Gameplay сцену
2. Загрузить GameplayState
3. Запуск GameplayEntryPoint.Run()
4. После завершения → LoadAndStartMainMenu() или следующий уровень
```

---

## Работа с UI (UIManager)

`GameplayUIManager` наследует `UIManager` и управляет всеми UI-элементами сцены.

### Архитектура UI

```
UIRootView (корневой Canvas)
├── Screen (активный экран) — ScreenGameplay
├── Popups (стек попапов) — PopupPause, PopupSettings, PopupLose, PopupFinish
└── Panels (постоянные панели) — 
    ├── PanelGateWave
    ├── PanelBuild
    ├── PanelActions
    ├── PanelConfirmation
    ├── PanelTowerAction
    └── PanelTowerPlacement
```

### Управление через FSM

Панели показываются/скрываются автоматически, основываясь на состояниях FSM:

```csharp
_fsmGameplay.Fsm.StateCurrent.Subscribe(newState => {
    if (newState is FsmStateBuildBegin)
        rootUI.ShowPanel<PanelBuildViewModel>();
    if (newState is FsmStateBuildEnd)
        rootUI.HidePanel<PanelBuildViewModel>();
});

_fsmTower.Fsm.StateCurrent.Subscribe(newState => {
    if (newState is FsmTowerSelected)
        rootUI.ShowPanel<PanelTowerActionViewModel>();
    if (newState is FsmTowerNone)
        rootUI.HidePanel<PanelTowerActionViewModel>();
});
```

При открытии попапов игра ставится на паузу (`FsmStateGamePause`), при закрытии — возобновляется.

---

## Заключение и принципы

### Ключевые принципы архитектуры

1. **Dependency Injection** — всё, что можно, регистрируется в контейнере
2. **Разделение ответственности** — сервисы (бизнес-логика), команды/запросы (изменение/чтение), вьюхи (отображение)
3. **Реактивность** — изменения данных автоматически обновляют UI
4. **Иерархия контейнеров** — корневые зависимости доступны на всех сценах
5. **CQRS** — явное разделение команд и запросов
6. **FSM** — явное управление состояниями игры
7. **Proxy для состояний** — реактивные обёртки над POCO-данными

### Поток данных

```
UI Event → Command → CommandHandler → GameplayStateProxy (изменение) 
    → ObservableCollection/ReactiveProperty (уведомление) 
    → Service → ViewModel → View (Binder)
```

### Типичный жизненный цикл объекта (например, башни)

1. `CommandPlaceTower` — DTO с параметрами
2. `CommandPlaceTowerHandler` — создаёт `TowerEntityData`, оборачивает в `TowerEntity`
3. `TowerEntity` добавляется в `ObservableList<TowerEntity>`
4. `TowersService` (подписан на добавление) — создаёт `TowerViewModel`
5. `TowerViewModel` связывается с визуальным представлением через Binder
6. При удалении — `ObservableList` уведомляет сервис, `TowerViewModel` дизпозится
