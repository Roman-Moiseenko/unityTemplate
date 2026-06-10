# 10. Поток загрузки игры (GameEntryPoint)

## Автостарт

```csharp
[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
public static void AutostartGame()
{
    // 1. Настройка FPS (30), отключение тайм-аута экрана
    Application.targetFrameRate = 30;
    Screen.sleepTimeout = SleepTimeout.NeverSleep;
    
    // 2. Создание GameEntryPoint._instance
    // 3. Запуск RunGame()
}
```

## Полный Flow загрузки

```
GameEntryPoint.AutostartGame()
    │
    ├── 0. Application.targetFrameRate = 30
    │      Screen.sleepTimeout = SleepTimeout.NeverSleep
    │
    ├── 1. Создание корневого контейнера _rootContainer
    │      ├── UIRootView (префаб Resources/"UIRoot")
    │      ├── ImageManagerBinder (префаб Resources/"ImageManager")
    │      ├── StorageManager (кэш текстур)
    │      ├── ISettingsProvider (SettingsProviderWeb) .AsSingle()
    │      ├── ICommandProcessor (CommandProcessor) .AsSingle()
    │      ├── IQueryProcessor (QueryProcessor) .AsSingle()
    │      ├── IGameStateProvider (WebGameStateProvider) .AsSingle()
    │      ├── CommandSaveGameStateHandler (регистрация)
    │      ├── AdService .AsSingle()
    │      ├── ResourceService .AsSingle()
    │      └── GenerateService .AsSingle()
    │
    └── LoadFirstBoot()
        │
        ├── 2. Показать loading-экран
        ├── 3. Загрузить Boot сцену → MainMenu сцену
        │
        ├── 4. Проверка веб-доступа (CheckWebAvailable)
        │      └── LoadingState.Loaded → true/false
        │
        ├── 5. Загрузка настроек (LoadSettingsState)
        │      ├── Регистрация нового пользователя (userId, userToken) при первом запуске
        │      ├── Загрузка локальных настроек из PlayerPrefs
        │      ├── Если есть веб — загрузка и сравнение DateVersion
        │      └── LoadingState.Loaded → true
        │
        ├── 6. Загрузка данных игрока (LoadGameState)
        │      ├── Загрузка из PlayerPrefs (или DefaultGameState)
        │      ├── Если есть веб — загрузка и сравнение DateVersion
        │      └── LoadingState.Loaded → true
        │
        ├── 7. Загрузка настроек игры (LoadGameSettings)
        │      └── LoadingState.Loaded → true
        │
        ├── 8. Регистрация команд, зависимых от LoadGameState
        │      ├── CommandSpendHardCurrencyHandler
        │      └── CommandAddHardCurrencyHandler
        │
        └── 9. Запуск MainMenuEntryPoint.Run(mainMenuContainer, null)
               │
               ├── Регистрация сервисов MainMenu
               │   ├── InventoryService
               │   ├── ChestService
               │   ├── TowerCardPlanService
               │   └── SkillCardPlanService
               │
               ├── Создание колоды Inventory (если пусто — CommandCreateInventory)
               ├── Открытие UI (MainScreen, ScreenPlay)
               │
               └── Подписка на MainMenuExitParams
                      │
                      └── Если TargetSceneEnterParams.SceneName == Scenes.GAMEPLAY
                            ↓
                      LoadAndStartGameplay(GameplayEnterParams)
                        │
                        ├── 1. Dispose старого _cachedSceneContainer
                        ├── 2. LoadScene(BOOT) → LoadScene(GAMEPLAY)
                        ├── 3. LoadGameplayState()
                        │      ├── Из PlayerPrefs (или DefaultGameplayState)
                        │      └── Создание GameplayStateProxy
                        │
                        ├── 4. Создание gameplayContainer = new DIContainer(_rootContainer)
                        │
                        ├── 5. GameplayEntryPoint.Run(gameplayContainer, enterParams)
                        │      └── (см. 11_Gameplay.md)
                        │
                        └── 6. Подписка на GameplayExitParams
                               │
                               ├── LoadAndStartMainMenu(gameplayExitParams.MainMenuEnterParams)
                               │   └── Возврат в меню с наградами
                               │
                               └── Если SaveGameplay == false → ResetGameplayState()
```

## Сцены

| Имя сцены | Назначение |
|-----------|------------|
| `Boot` | Промежуточная сцена для перехода (загрузка и выгрузка ресурсов) |
| `MainMenu` | Главное меню (выбор карт, инвентарь, сундуки) |
| `Gameplay` | Игровая сцена |

## CachedSceneContainer

При каждом переходе между сценами:
1. Старый `_cachedSceneContainer` дизпозится (все зарегистрированные на сцену объекты очищаются)
2. Создаётся новый контейнер, наследующий корневой контейнер
3. Все сервисы сцены регистрируются в новом контейнере

```csharp
private void LoadAndStartGameplay(GameplayEnterParams enterParams)
{
    // Очистка старого контейнера
    _cachedSceneContainer?.DisposeSceneDisposables();
    
    // Создание нового
    _cachedSceneContainer = new DIContainer(_rootContainer);
    
    // Загрузка сцены
    SceneManager.LoadScene(Scenes.BOOT);
    SceneManager.LoadScene(Scenes.GAMEPLAY);
    
    // Загрузка состояния
    _gameStateProvider.LoadGameplayState()
        .Subscribe(loadingState => {
            if (loadingState.IsLoaded)
            {
                // Запуск EntryPoint
                var gameplayEntryPoint = FindObjectOfType<GameplayEntryPoint>();
                gameplayEntryPoint.Run(_cachedSceneContainer, enterParams);
            }
        });
}
```

## MainMenuExitParams → GameplayEnterParams

```csharp
public class MainMenuExitParams
{
    public SceneEnterParams TargetSceneEnterParams;
}

public class GameplayEnterParams : SceneEnterParams
{
    public int MapId;
    public float GameSpeed;
    public TypeGameplay TypeGameplay;
    public List<TowerCardData> Towers;          // Колода башен
    public List<SkillCardData> Skills;          // Колода скиллов
    public HeroCardData HeroCard;               // Карточка героя
    public GameplayBoosters GameplayBoosters;    // Бустеры
}

public class GameplayExitParams
{
    public MainMenuEnterParams MainMenuEnterParams;  // Награды и результаты
    public bool SaveGameplay;                         // Сохранить сессию
}
```

---

*Читайте далее: [11_Gameplay.md](11_Gameplay.md)*
