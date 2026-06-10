# 19. Система статистики геймплея

## Назначение

`StatisticGame` собирает статистику текущей игровой сессии. Данные хранятся в `GameplayState.StatisticGameData` и после завершения сессии передаются в `MainMenuEnterParams` для отображения результатов.

## StatisticGame (Proxy)

```csharp
public class StatisticGame : IDisposable
{
    public StatisticGameData Origin;
    
    public ReactiveProperty<int> TotalKills;          // Всего убито мобов
    public ReactiveProperty<int> TotalDamage;         // Нанесено урона
    public ReactiveProperty<int> MaxWave;             // Максимальная волна
    public ReactiveProperty<int> TowerPlacement;      // Построено башен
    public ReactiveProperty<int> TowerDelete;         // Удалено башен
    public ReactiveProperty<int> TowerLevelUp;        // Улучшено башен
    public ReactiveProperty<int> UseSkillCount;       // Использовано скиллов
    public ReactiveProperty<int> UseRepairCristal;    // Воскрешений за кристаллы
    public ReactiveProperty<int> UseRepairAd;         // Воскрешений за рекламу
    public ReactiveProperty<int> UseSpeedBooster;     // Использовано ускорений
    public ReactiveProperty<int> DamageToCastle;      // Урона по замку (от мобов)
}
```

## StatisticGameData (POCO)

```csharp
public class StatisticGameData
{
    public int TotalKills;
    public int TotalDamage;
    public int MaxWave;
    public int TowerPlacement;
    public int TowerDelete;
    public int TowerLevelUp;
    public int UseSkillCount;
    public int UseRepairCristal;
    public int UseRepairAd;
    public int UseSpeedBooster;
    public int DamageToCastle;
}
```

## Как собирается статистика

Статистика обновляется сервисами в процессе игры:

```csharp
// WaveService — при смерти моба
gameplayState.StatisticGame.TotalKills.Value++;

// DamageService — при нанесении урона
gameplayState.StatisticGame.TotalDamage.Value += damageValue;

// TowersService — при постройке башни
gameplayState.StatisticGame.TowerPlacement.Value++;

// CastleService — при получении урона замком
gameplayState.StatisticGame.DamageToCastle.Value += damage;
```

## Использование статистики

### Отображение в UI

```csharp
public class ResultStatsBinder : PanelBinder
{
    [SerializeField] private TextMeshProUGUI _killsText;
    [SerializeField] private TextMeshProUGUI _damageText;
    [SerializeField] private TextMeshProUGUI _wavesText;
    
    public void Bind(StatisticGame statistic)
    {
        statistic.TotalKills.Subscribe(v => _killsText.text = v.ToString());
        statistic.TotalDamage.Subscribe(v => _damageText.text = v.ToString());
    }
}
```

### Передача в MainMenuEnterParams

После завершения сессии статистика упаковывается в `MainMenuEnterParams`:

```csharp
var exitParams = new GameplayExitParams
{
    MainMenuEnterParams = new MainMenuEnterParams
    {
        KillsMob = gameplayState.StatisticGame.TotalKills.Value,
        LastWave = gameplayState.CurrentWave.Value,
        SoftCurrency = gameplayState.SoftCurrency.Value,
        // ...
    }
};
```

---

*Читайте далее: [20_Types_Enums.md](20_Types_Enums.md)*
