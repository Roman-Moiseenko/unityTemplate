# 17. Инвентарь, карточки и колоды

## Структура инвентаря

Инвентарь игрока хранится в `GameState.Inventory` и содержит все карточки (башен, скиллов, героев) и планы (улучшения).

```csharp
public class InventoryRootData
{
    public int GlobalInventoryId;                     // Глобальный ID
    public List<InventoryItemData> Items;            // Все предметы
}
```

### InventoryItemData (базовый класс)

```csharp
public class InventoryItemData
{
    public int UniqueId;                             // Уникальный ID предмета
    public string ConfigId;                          // ID конфига
    public TypeInventoryItem TypeItem;               // Тип предмета
    public int Level;                                // Уровень
    public int Count;                                // Количество
}
```

## Типы предметов

| Тип | Назначение |
|-----|-----------|
| `TowerCard` | Карточка башни (можно разместить на поле) |
| `SkillCard` | Карточка скилла (можно экипировать) |
| `HeroCard` | Карточка героя |
| `TowerPlan` | План улучшения башни |
| `SkillPlan` | План улучшения скилла |
| `Chest` | Сундук с наградами |
| `Currency` | Валюта |

## Карточки башен (TowerCard)

```csharp
public class TowerCardData : InventoryItemData
{
    // Уровень карточки (1, 2, 3...), влияет на уровень башни при размещении
    public int Level;
    
    // Параметры улучшения (заполняются из TowerSettings при получении карты)
    public List<ParameterData> Parameters;
}
```

**Получение карты башни:**
- Покупка в магазине
- Награда за прохождение волны
- Открытие сундука
- Слияние двух одинаковых карт (улучшение уровня)

## Карточки скиллов (SkillCard)

```csharp
public class SkillCardData : InventoryItemData
{
    public int Level;
    public float Cooldown;          // Перезарядка
    public float Damage;            // Урон
}
```

## Карточки героев (HeroCard)

```csharp
public class HeroCardData : InventoryItemData
{
    public int Level;
    public int Health;
    public List<HeroBuffData> Buffs;   // Баффы, которые даёт герой
}
```

## Колоды (Card Plan)

Перед началом игры игрок собирает колоду:
- **Колода башен** — 3 карты, которые можно размещать на поле
- **Колода скиллов** — 2 карты, которые можно применять
- **Герой** — 1 карта (опционально)

```csharp
public class CardPlanData
{
    public int UniqueId;
    public string ConfigId;
}

public class CardPlanService
{
    // Текущая колода башен
    public List<CardPlanData> TowerPlans;
    
    // Текущая колода скиллов
    public List<CardPlanData> SkillPlans;
    
    // Текущий герой
    public CardPlanData HeroPlan;
}
```

## Сундуки (Chests)

```csharp
public class ContainerChestsData
{
    public int CountOpen;              // Количество открытых сундуков
    public List<ChestData> Chests;     // Доступные сундуки
}

public class ChestData
{
    public TypeChest TypeChest;        // Тип сундука
    public int Count;                  // Количество
    public bool IsOpened;              // Открыт ли
    public List<RewardData> Rewards;   // Награды внутри
}
```

## Инициализация инвентаря

При первом запуске игры, если инвентарь пуст, создаётся начальный набор через `CommandCreateInventory`:

```csharp
public class CommandCreateInventory : ICommand
{
    // Создаёт начальные карточки башен, скиллов и героев
    // Наполнение определяется в InventoryInitialSettings
}
```

## Команды инвентаря

| Команда | Описание |
|---------|---------|
| `CommandCreateInventory` | Создать начальный инвентарь |
| `CommandAddItem` | Добавить предмет |
| `CommandRemoveItem` | Удалить предмет |
| `CommandUpgradeCard` | Улучшить карточку |
| `CommandOpenChest` | Открыть сундук |
| `CommandMergeCards` | Слить две карты в одну |

---

*Читайте далее: [18_Currency.md](18_Currency.md)*
