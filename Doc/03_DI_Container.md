# 3. DI-контейнер (Inversion of Control)

## DIContainer

`DI.DIContainer` — самописный IoC-контейнер, основанный на концепции вложенных контейнеров (parent scoping). Позволяет регистрировать и разрешать зависимости с управлением жизненным циклом.

### Базовые возможности

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

// Проверка наличия
bool exists = container.HasRegistered<IService>();
```

### Управление жизненным циклом

```csharp
// Регистрация disposable-объектов, которые будут уничтожены при выходе со сцены
container.RegisterDisposableOnSceneExit(myDisposable);

// При уничтожении сцены все такие объекты дизпозятся в обратном порядке
container.DisposeSceneDisposables();
```

### Вложенные контейнеры

Контейнер строит иерархию:

```
RootContainer (живёт всё время)
    ├── SceneContainer (MainMenu)
    └── SceneContainer (Gameplay)
```

При разрешении зависимости, если она не найдена в текущем контейнере, поиск идёт в родительском (и далее — вплоть до корневого).

### DIEntry

`DIEntry` — вспомогательный класс для объявления фабрик и singleton:

```csharp
public class DIEntry<T>
{
    public static DIEntry<T> Create(Func<DIContainer, T> factory);
    public DIEntry<T> AsSingle();              // Singleton
    public DIEntry<T> WithTag(string tag);     // С тегом
}
```

### Пример иерархии контейнеров в проекте

```csharp
// Корневой контейнер (GameEntryPoint)
_rootContainer = new DIContainer();
_rootContainer.RegisterFactory<ICommandProcessor>(c => new CommandProcessor()).AsSingle();
_rootContainer.RegisterFactory<IQueryProcessor>(c => new QueryProcessor()).AsSingle();
_rootContainer.RegisterFactory<IGameStateProvider>(c => new WebGameStateProvider()).AsSingle();
_rootContainer.RegisterFactory<ISettingsProvider>(c => new SettingsProviderWeb()).AsSingle();
// ... глобальные сервисы

// Контейнер сцены Gameplay
var gameplayContainer = new DIContainer(_rootContainer);
gameplayContainer.RegisterFactory<WaveService>(c => new WaveService(...)).AsSingle();
gameplayContainer.RegisterFactory<TowersService>(c => new TowersService(...)).AsSingle();
// ... сервисы сцены
```

---

*Читайте далее: [04_CQRS.md](04_CQRS.md)*
