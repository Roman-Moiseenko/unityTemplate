# 6. MVVM — Model-View-ViewModel

## Концепция MVVM в проекте

- **Model** — данные (`EntityData`, `GameState`)
- **ViewModel** — реактивная обёртка, преобразующая модель в данные для UI
- **View** — Binder (MonoBehaviour), подписывающийся на ViewModel и обновляющий Unity-компоненты

Все реакции реализованы через **R3** (`ReactiveProperty`, `Subject`, `Observable`) и **ObservableCollections**.

## Структура MVVM-фреймворка

```
MVVM/
├── UI/                    # Базовые UI-компоненты
│   ├── UIRootViewModel.cs
│   ├── UIRootBinder.cs
│   ├── PanelBinder.cs
│   ├── PopupBinder.cs
│   ├── WindowBinder.cs
│   ├── DropdownBinder.cs
│   └── UIManager.cs
├── CMD/                   # CQRS
│   ├── ICommand.cs
│   ├── IQuery.cs
│   ├── CommandProcessor.cs
│   └── QueryProcessor.cs
├── FSM/                   # Конечные автоматы
│   ├── FSM.cs
│   └── FsmProxy.cs
└── Storage/               # Хранилища
    ├── PoolMono.cs
    └── StorageManager.cs
```

## Базовые UI-компоненты

| Класс | Назначение |
|-------|-----------|
| `UIRootViewModel` | Корневая ViewModel (содержит Screen, Popups, Panels) |
| `UIRootBinder` | Биндер корневого UI (Canvas) |
| `PanelBinder` | Базовый класс для панелей |
| `PopupBinder` | Базовый класс для попапов |
| `WindowBinder` | Базовый класс для окон |
| `DropdownBinder` | Базовый класс для выпадающих списков |
| `UIManager` | Менеджер UI (открытие/закрытие экранов, попапов, панелей) |

## Пример связки ViewModel → Binder

### ViewModel

```csharp
public class PanelActionsViewModel : PanelViewModel
{
    public ReactiveProperty<bool> IsVisible = new(false);
    public Subject<Unit> OnBuildClicked = new();
    public ReactiveProperty<string> TitleText = new("Actions");
    public ReactiveProperty<bool> IsBuildButtonEnabled = new(true);
    
    public PanelActionsViewModel(UIManager uiManager, DIContainer container) 
        : base(uiManager, container)
    {
        var fsmGameplay = container.Resolve<FsmGameplay>();
        
        // Подписка на FSM для авто-отображения
        fsmGameplay.Fsm.StateCurrent.Subscribe(state => {
            IsVisible.Value = state is FsmStateBuildBegin or FsmStateBuild;
        });
    }
}
```

### Binder

```csharp
public class PanelActionsBinder : PanelBinder
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private Button _buildButton;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private CanvasGroup _canvasGroup;

    private void Bind(PanelActionsViewModel viewModel)
    {
        // Подписка на видимость
        viewModel.IsVisible.Subscribe(v => _panel.SetActive(v));
        
        // Подписка на текст
        viewModel.TitleText.Subscribe(t => _titleText.text = t);
        
        // Подписка на доступность кнопки
        viewModel.IsBuildButtonEnabled.Subscribe(enabled => {
            _buildButton.interactable = enabled;
            _canvasGroup.alpha = enabled ? 1f : 0.5f;
        });
        
        // Обработка клика
        _buildButton.onClick.AddListener(() => viewModel.OnBuildClicked.OnNext(Unit.Default));
    }
}
```

## Типы ViewModel

### PanelViewModel

Базовый класс для всех панелей UI:

```csharp
public abstract class PanelViewModel : IDisposable
{
    protected UIManager UIManager;
    protected DIContainer Container;
    
    public virtual void Dispose() { }
}
```

### PopupViewModel

Базовый класс для попапов (ставят игру на паузу):

```csharp
public abstract class PopupViewModel : PanelViewModel
{
    public Subject<Unit> OnClose = new();
    
    public virtual void Open() { }
    public virtual void Close() { }
}
```

### ScreenViewModel

Базовый класс для экранов:

```csharp
public abstract class ScreenViewModel : IDisposable
{
    public virtual void Dispose() { }
}
```

## Примеры ViewModel в проекте

| ViewModel | Тип | Назначение |
|-----------|-----|-----------|
| `UIGameplayRootViewModel` | UIRootViewModel | Корневая VM сцены Gameplay |
| `PanelBuildViewModel` | PanelViewModel | Панель выбора башни для строительства |
| `PanelTowerActionViewModel` | PanelViewModel | Действия с выбранной башней |
| `PanelGateViewModel` | PanelViewModel | Индикатор волны (таймер) |
| `PanelConfirmationViewModel` | PanelViewModel | Подтверждение действия |
| `PopupPauseViewModel` | PopupViewModel | Пауза |
| `PopupSettingsViewModel` | PopupViewModel | Настройки |
| `PopupLoseViewModel` | PopupViewModel | Поражение |
| `PopupFinishViewModel` | PopupViewModel | Победа |
| `ScreenGameplayViewModel` | ScreenViewModel | Основной экран игры |
| `WorldGameplayRootViewModel` | IDisposable | Корневая VM для мира (содержит все Towers/Mobs/...) |

## Регистрация ViewModel

```csharp
// GameplayViewModelsRegistrations.Register(viewContainer)
public static void Register(DIContainer container)
{
    var rootUI = new UIGameplayRootViewModel(container.Resolve<UIManager>(), container);
    container.RegisterInstance<UIGameplayRootViewModel>(rootUI);
    
    var worldRoot = new WorldGameplayRootViewModel(container);
    container.RegisterInstance<WorldGameplayRootViewModel>(worldRoot);
    
    // Регистрация Subject для кликов
    var clickWorldEntity = new Subject<Vector2>();
    container.RegisterInstance<Subject<Vector2>>("CLICK_WORLD_ENTITY", clickWorldEntity);
}
```

## Object Pooling для Binder

Для объектов, которые часто создаются/удаляются (мобы, выстрелы), используется Object Pool:

```csharp
public class PoolMono<T> where T : MonoBehaviour
{
    public T Get();                     // Взять из пула (или создать)
    public void Return(T obj);          // Вернуть в пул
    public void Clear();                // Очистить пул
}
```

---

*Читайте далее: [07_GameState.md](07_GameState.md)*
