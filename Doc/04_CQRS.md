# 4. CQRS — Command & Query Pattern

## Общая концепция

Архитектура разделяет операции на два типа:
- **Commands** — изменяют состояние (с автосохранением)
- **Queries** — читают данные (без изменения состояния)

## Команды (Commands)

### Интерфейсы

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

### Пример команды

```csharp
// Команда (DTO с данными)
public class CommandPlaceTower : ICommand
{
    public readonly string ConfigId;
    public readonly Vector2Int Position;
    public Vector2Int Placement { get; set; }
}

// Обработчик — содержит бизнес-логику
public class CommandPlaceTowerHandler : ICommandHandler<CommandPlaceTower>
{
    private readonly GameplayStateProxy _gameplayState;

    public bool Handle(CommandPlaceTower command)
    {
        var entityData = new TowerEntityData
        {
            UniqueId = _gameplayState.CreateEntityID(),
            ConfigId = command.ConfigId,
            Position = command.Position,
        };
        var towerEntity = new TowerEntity(entityData);
        _gameplayState.Towers.Add(towerEntity);
        return true;
    }
}
```

### Два типа CommandProcessor

| Процессор | Сохраняет | Использование |
|-----------|-----------|---------------|
| `CommandProcessor` | `SaveGameState()` + `SaveSettingsState()` | Глобальные команды (валюта, инвентарь) |
| `CommandProcessorGameplay` | `SaveGameplayState()` | Игровые команды (построить башню, создать моба) |

### Список команд

**Gameplay команды:**
- `CommandPlaceTower` — разместить башню
- `CommandDeleteTower` — удалить башню
- `CommandMoveTower` — переместить башню
- `CommandTowerLevelUp` — улучшить башню
- `CommandReplaceTower` — заменить башню
- `CommandCreateLevel` — создать уровень (карту)
- `CommandCreateInfinity` — создать бесконечный режим
- `CommandMobGenerate` — создать моба
- `CommandMobRemove` — удалить моба
- `CommandCreateRoad` — создать дорогу
- `CommandDeleteRoad` — удалить дорогу
- `CommandCreateGround` — создать клетку земли
- `CommandCreateCastle` — создать замок
- `CommandDamageCastle` — нанести урон замку
- `CommandCreateReward` — создать награду
- `CommandAddWarrior` — добавить воина
- `CommandRemoveWarrior` — убрать воина
- `CommandSkillApply` — применить скилл
- `CommandSaveGameState` — сохранить состояние

**Глобальные команды:**
- `CommandSpendHardCurrency` — потратить кристаллы
- `CommandAddHardCurrency` — добавить кристаллы
- `CommandAddSoftCurrency` — добавить золото
- `CommandCreateInventory` — создать инвентарь
- `CommandOpenChest` — открыть сундук

## Запросы (Queries)

### Интерфейсы

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

### Пример запроса

```csharp
public class QueryInfoWave : IQuery
{
    public string ConfigId;
    public int WaveNumber;
    public int AddWave;
}

public class QueryInfoWaveHandler : IQueryHandler<QueryInfoWave>
{
    public object Handle(QueryInfoWave query, ISettingsProvider settingsProvider)
    {
        var mapsSettings = settingsProvider.GameSettings.MapsSettings;
        var map = mapsSettings.Maps.First(m => m.ConfigId == query.ConfigId);
        var waveIndex = query.WaveNumber + query.AddWave;
        if (waveIndex >= map.Waves.Count) return null;
        return map.Waves[waveIndex];
    }
}
```

### Список запросов

- `QueryInfoWave` — получить информацию о волне
- `QueryInfoTower` — получить параметры башни из настроек
- `QueryInfoSkill` — получить параметры скилла из настроек

## Принцип работы

1. **Регистрация** — все обработчики регистрируются в процессоре при загрузке сцены
2. **Выполнение** — вызов `Process()` или `Request()` передаёт команду/запрос нужному обработчику
3. **Автосохранение** — после успешной обработки команды состояние автоматически сохраняется

```csharp
// Регистрация обработчиков
_commandProcessor.RegisterHandler<CommandPlaceTower>(new CommandPlaceTowerHandler(gameplayState));
_commandProcessor.RegisterHandler<CommandDeleteTower>(new CommandDeleteTowerHandler(gameplayState));

// Выполнение команды
var command = new CommandPlaceTower { ConfigId = "tower_01", Position = new Vector2Int(3, 4) };
bool success = _commandProcessor.Process(command, autoSave: true);

// Выполнение запроса
var query = new QueryInfoWave { ConfigId = "map_01", WaveNumber = 5 };
var waveData = _queryProcessor.Request(query) as WaveData;
```

---

*Читайте далее: [05_FSM.md](05_FSM.md)*
