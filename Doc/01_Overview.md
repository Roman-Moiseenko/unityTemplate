# 1. Общий обзор архитектуры

## Назначение

Проект представляет собой **Tower Defense** игру на Unity. В основе архитектуры лежат пять ключевых паттернов: DI, CQRS, FSM, MVVM и Proxy с реактивными расширениями.

## Ключевые паттерны

| Паттерн | Назначение | Реализация |
|---------|------------|------------|
| **DI (Dependency Injection)** | Управление зависимостями, слабая связанность кода | Самописный `DIContainer` с поддержкой вложенных контейнеров (parent scoping) |
| **CQRS (Command Query Responsibility Segregation)** | Разделение операций изменения состояния (команды) и чтения данных (запросы) | `ICommand` / `ICommandHandler` + `IQuery` / `IQueryHandler` с процессорами |
| **FSM (Finite State Machine)** | Явное управление состояниями игры и объектов | Базовый `FSM` + `FsmProxy` для реактивной подписки |
| **MVVM (Model-View-ViewModel)** | Связывание данных с UI через реактивные свойства | ViewModel (R3) → Binder (MonoBehaviour) → Unity-компоненты |
| **Proxy + Reactive Extensions** | Реактивные обёртки над POCO-данными с автосинхронизацией | Библиотеки `R3` и `ObservableCollections` |

## Основные принципы

1. **Всё через DI-контейнер** — зависимости регистрируются и разрешаются через контейнер
2. **Разделение ответственности** — сервисы (бизнес-логика), CQRS (изменение/чтение данных), View (отображение)
3. **Реактивность** — изменение данных автоматически обновляет UI через подписки R3
4. **Иерархия контейнеров** — корневые зависимости (глобальные сервисы) доступны на всех сценах
5. **CQRS** — команды меняют состояние, запросы читают; процессоры управляют автосохранением
6. **FSM** — каждое состояние игры моделируется конечным автоматом
7. **Proxy-обёртки** — POCO-данные оборачиваются в реактивные Proxy с автосинхронизацией

## Жизненный цикл сцены

```
GameEntryPoint (корень)
    ↓ LoadScene()
    ↓
EntryPoint сцены (MainMenuEntryPoint / GameplayEntryPoint)
    ↓ Run(container, enterParams)
    ↓
Registrations.Register() — регистрация сервисов, CQRS, FSM
    ↓
ViewModelsRegistrations.Register() — регистрация ViewModel
    ↓
InitWorld() / InitUI() — инициализация
    ↓
Подписка на exitParams → переход на другую сцену
```

## Поток данных (общий)

```
User Input (клик/кнопка)
    → ViewModel: Subject.OnNext()
    → Service: бизнес-логика, вызов Command
    → CommandHandler: создание/изменение EntityData
    → GameplayStateProxy (ObservableList / ReactiveProperty)
    → Service (подписка): создание ViewModel
    → ViewModel (R3.Subscribe)
    → Binder (обновление Unity-компонентов)
    → Визуальное изменение на экране
```

## Три уровня состояния

| Уровень | POCO | Proxy | Хранилище |
|---------|------|-------|-----------|
| **GameState** | `GameState` | `GameStateProxy` | PlayerPrefs + сервер |
| **GameplayState** | `GameplayState` | `GameplayStateProxy` | PlayerPrefs (сессия) |
| **GameSettingsState** | `GameSettingsState` | `GameSettingsStateProxy` | PlayerPrefs + сервер |

## Основные библиотеки

- **R3** — реактивные свойства и подписки (аналог UniRx)
- **ObservableCollections** — реактивные коллекции (ObserveAdd, ObserveRemove)
- **Newtonsoft.Json** — сериализация состояния
- **Cysharp.Threading.Tasks** — async/await поддержка (UniTask)

---

*Читайте далее: [02_Project_Structure.md](02_Project_Structure.md)*
