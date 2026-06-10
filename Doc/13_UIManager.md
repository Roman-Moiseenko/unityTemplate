# 13. Система UI и UIManager

## UIManager — базовая архитектура

`UIManager` — абстрактный класс, наследующий `IDisposable`, управляющий всеми UI-элементами сцены.

```csharp
public abstract class UIManager : IDisposable
{
    public ScreenViewModel CurrentScreen;               // Текущий экран
    public ObservableStack<PopupViewModel> CurrentPopup; // Стек открытых попапов
    public PanelViewModel CurrentPanel;                 // Текущая панель
    public Dictionary<string, PanelViewModel> ScenePanels = new(); // Все панели сцены
    
    // Управление
    public void ShowScreen(ScreenViewModel screen);
    public void ShowPanel<T>(object param = null) where T : PanelViewModel;
    public void HidePanel<T>() where T : PanelViewModel;
    public void ShowPopup<T>(object param = null) where T : PopupViewModel;
    public void HidePopup();
}
```

**GameplayUIManager** — реализация для сцены Gameplay, наследует `UIManager`.

## Структура UI

```
UIRootView (корневой Canvas)
│
├── Screen (активный экран)
│   └── ScreenGameplay — основной экран игры
│
├── Popups (стеки попапов — поверх экрана, ставят игру на паузу)
│   ├── PopupPause — пауза
│   ├── PopupSettings — настройки
│   ├── PopupLose — поражение
│   ├── PopupFinish — победа
│   └── PopupMapChange — смена карты (при строительстве дороги)
│
└── Panels (постоянные панели, не блокируют игру)
    ├── PanelGateWave — индикатор волны (таймер до следующей волны)
    ├── PanelBuild — панель строительства (выбор башни)
    ├── PanelActions — панель действий (строить, убрать, ускорить)
    ├── PanelConfirmation — подтверждение действия
    ├── PanelTowerAction — действия с выбранной башней (улучшить, удалить, переместить)
    ├── PanelTowerPlacement — панель при размещении башни
    └── PanelCardSkills — панель скиллов
```

## Архитектура ViewModel → Binder

Каждый UI элемент состоит из двух частей:

### 1. ViewModel (логика)

Наследует `PanelViewModel` / `PopupViewModel` / `ScreenViewModel`:

```csharp
public class PanelGateViewModel : PanelViewModel
{
    public ReactiveProperty<bool> IsVisible;
    public ReactiveProperty<float> TimeOutNewWave;  // Прогресс таймера (0..1)
    public ReactiveProperty<int> CurrentWave;
    public ReactiveProperty<int> CountWaves;
    
    public PanelGateViewModel(UIManager uiManager, DIContainer container) 
        : base(uiManager, container)
    {
        var waveService = container.Resolve<WaveService>();
        
        // Подписка на таймер волны
        waveService.TimeOutNewWaveValue.Subscribe(v => TimeOutNewWave.Value = v);
        
        // Подписка на текущую волну
        var gameplayState = container.Resolve<GameplayStateProxy>();
        gameplayState.CurrentWave.Subscribe(w => CurrentWave.Value = w);
        CountWaves.Value = gameplayState.CountWaves;
    }
}
```

### 2. Binder (визуальное представление)

MonoBehaviour, наследующий `PanelBinder` / `PopupBinder`:

```csharp
public class PanelGateBinder : PanelBinder
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private Image _timerFill;
    [SerializeField] private TextMeshProUGUI _waveText;
    
    private void Bind(PanelGateViewModel viewModel)
    {
        // Видимость
        viewModel.IsVisible.Subscribe(v => _panel.SetActive(v));
        
        // Таймер (заполнение шкалы)
        viewModel.TimeOutNewWave.Subscribe(v => _timerFill.fillAmount = v);
        
        // Текст волны
        Observable.CombineLatest(
            viewModel.CurrentWave, 
            viewModel.CountWaves, 
            (current, total) => $"Wave {current}/{total}"
        ).Subscribe(text => _waveText.text = text);
    }
}
```

## Управление через FSM

Панели показываются/скрываются автоматически по состояниям FSM:

```csharp
// Панель строительства
_fsmGameplay.Fsm.StateCurrent.Subscribe(newState => {
    if (newState is FsmStateBuildBegin)
        rootUI.ShowPanel<PanelBuildViewModel>();
    if (newState is FsmStateBuildEnd)
        rootUI.HidePanel<PanelBuildViewModel>();
});

// Панель действий с башней
_fsmTower.Fsm.StateCurrent.Subscribe(newState => {
    if (newState is FsmTowerSelected)
        rootUI.ShowPanel<PanelTowerActionViewModel>();
    if (newState is FsmTowerNone)
        rootUI.HidePanel<PanelTowerActionViewModel>();
});

// Панель подтверждения
_fsmTower.Fsm.StateCurrent.Subscribe(newState => {
    if (newState is FsmTowerDelete)
        rootUI.ShowPanel<PanelConfirmationViewModel>();
});

// Управление попапами
_fsmGameplay.Fsm.StateCurrent.Subscribe(newState => {
    if (newState is FsmStateGamePause)
    {
        rootUI.ShowPopup<PopupPauseViewModel>();
        Time.timeScale = 0;
    }
    if (newState is FsmStateGamePlay)
    {
        rootUI.HidePopup();
        Time.timeScale = gameSpeed;
    }
});
```

## Экран игры (ScreenGameplay)

`ScreenGameplayViewModel` — корневой экран игровой сцены. Содержит подписки на все ключевые состояния:

- Количество жизней замка
- Количество валюты (золото, кристаллы)
- Прогресс волны
- Кнопки паузы и ускорения

## Регистрация UI

```csharp
// GameplayViewModelsRegistrations.Register()
public static void Register(DIContainer container)
{
    // Subject для кликов по миру
    var clickWorldEntity = new Subject<Vector2>();
    container.RegisterInstance("CLICK_WORLD_ENTITY", clickWorldEntity);
    
    // Subject для клика по башне
    var clickTower = new Subject<TowerEntity>();
    container.RegisterInstance("CLICK_TOWER", clickTower);
    
    // UIManager
    var uiManager = new GameplayUIManager();
    container.RegisterInstance<UIManager>(uiManager);
    
    // Корневая ViewModel
    var rootUI = new UIGameplayRootViewModel(uiManager, container);
    container.RegisterInstance(rootUI);
    
    // Мировая ViewModel
    var worldRoot = new WorldGameplayRootViewModel(container);
    container.RegisterInstance(worldRoot);
}
```

## Жизненный цикл UI

1. **Создание** — ViewModel регистрируется в контейнере
2. **Привязка** — Binder подписывается на ReactiveProperty ViewModel
3. **Работа** — ViewModel получает данные от сервисов, Binder обновляет визуал
4. **Скрытие** — FSM переключает состояние, UIManager скрывает панель
5. **Уничтожение** — при выходе со сцены все ViewModel дизпозятся

---

*Читайте далее: [14_ImageManager.md](14_ImageManager.md)*
