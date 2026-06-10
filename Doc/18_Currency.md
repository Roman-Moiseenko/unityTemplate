# 18. Игровые валюты и ресурсы

## Типы валют

| Валюта | Тип | Поле в GameState | Назначение |
|--------|-----|------------------|------------|
| **HardCurrency** | Премиум-валюта (кристаллы) | `GameState.HardCurrency` | Покупка бустеров, ускорение, воскрешение замка, премиум-предметы |
| **SoftCurrency** | Игровая валюта (золото) | `GameState.SoftCurrency` | Покупка карт, улучшение, открытие сундуков |
| **SoftCurrency Session** | Сессионное золото | `GameplayState.SoftCurrency` | Золото, заработанное в текущей сессии |

## HardCurrency (Кристаллы)

**Способы получения:**
- Покупка за реальные деньги
- Награда за прохождение карт
- Ежедневные задания
- Просмотр рекламы

**Траты:**
- Воскрешение замка (`GameplayService.RepairCristal()`)
- Ускорение таймера волны
- Покупка бустеров перед игрой
- Открытие сундуков

## SoftCurrency (Золото)

### Глобальное золото (GameState.SoftCurrency)

Хранится в `GameState` и используется в меню:

**Способы получения:**
- Награда за прохождение волн
- Продажа карт
- Открытие сундуков

**Траты:**
- Покупка карточек башен/скиллов
- Улучшение карт
- Покупка планов улучшения

### Сессионное золото (GameplayState.SoftCurrency)

Золото, заработанное в текущей игровой сессии. При завершении игры (победа/поражение) переводится в глобальное.

**Способы получения в сессии:**
- Убийство мобов (каждый моб даёт награду)
- Прогресс-награды (за достижение уровня прогресса)
- Награды за волны

## Команды управления валютой

```csharp
// Трата кристаллов
public class CommandSpendHardCurrency : ICommand
{
    public long Amount;
}

// Добавление кристаллов
public class CommandAddHardCurrency : ICommand
{
    public long Amount;
}

// Добавление золота
public class CommandAddSoftCurrency : ICommand
{
    public long Amount;
}
```

Все команды проверяют достаточность средств перед выполнением.

## Бустеры (GameplayBoosters)

Покупаются за кристаллы перед началом игры:

```csharp
public class GameplayBoosters
{
    public int DamagePercent;         // Бонус к урону (+%)
    public int CritChance;            // Шанс крита (+%)
    public int SpeedAttack;           // Скорость атаки (+%)
    public bool IsDoubleReward;       // Двойная награда
}
```

## ResourceService

Глобальный сервис для работы с ресурсами и подписками:

```csharp
public class ResourceService : IDisposable
{
    // Проверка подписок
    public bool HasSubscription(string subscriptionId);
    
    // Получение бустеров
    public GameplayBoosters GetBoosters();
    
    // Проверка доступности премиум-функций
    public bool IsPremiumAvailable();
}
```

---

*Читайте далее: [19_Statistics.md](19_Statistics.md)*
