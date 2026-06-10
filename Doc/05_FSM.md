# 5. FSM — Finite State Machine

## Базовая реализация (`MVVM.FSM.FSM`)

### Класс FSM

```csharp
public class FSM
{
    public FSMState StateCurrent { get; private set; }
    public FSMState PreviousState { get; private set; }
    public Dictionary<Type, FSMState> States = new();

    public void AddState(FSMState state);
    public void SetState<T>() where T : FSMState;  // Exit текущего → Enter нового
    public void Update();  // Вызывается из MonoBehaviour.Update()
}
```

### Базовое состояние

```csharp
public abstract class FSMState
{
    public abstract void Enter();        // Вызывается при входе в состояние
    public abstract bool Exit(FSMState next = null);  // Вызывается при выходе (может запретить)
    public abstract void Update();       // Вызывается каждый кадр
}
```

## FsmProxy — реактивная обёртка

`FsmProxy` оборачивает FSM и предоставляет `ReactiveProperty<FSMState>` для подписки через R3:

```csharp
public class FsmProxy
{
    public ReactiveProperty<FSMState> StateCurrent;
    public FSMState PreviousState;
    
    public void SetState<T>() where T : FSMState;
    public void Update();
}
```

## FsmWrapper — обёртка для конкретного автомата

Каждый автомат имеет свою обёртку, которая создаёт состояния, регистрирует их в FsmProxy и предоставляет удобные методы проверки:

```csharp
public class FsmGameplay
{
    public readonly FsmProxy Fsm;
    
    public bool IsStateGamePlay() { return Fsm.StateCurrent.Value is FsmStateGamePlay; }
    public bool IsStateBuilding() { return Fsm.StateCurrent.Value is FsmStateBuild or FsmStateBuildBegin; }
    public bool IsStatePause() { return Fsm.StateCurrent.Value is FsmStateGamePause; }
    public bool IsStateSkill() { return Fsm.StateCurrent.Value is FsmStateSelectSkill or FsmStateSetSkill; }
}
```

## Пять автоматов в проекте

| Автомат | Состояния | Назначение |
|---------|-----------|------------|
| `FsmGameplay` | `GamePause`, `GamePlay`, `SelectSkill`, `SetSkill`, `BuildBegin`, `Build`, `BuildEnd` | Общее состояние геймплея |
| `FsmTower` | `None`, `Selected`, `Delete`, `Placement`, `PlacementEnd` | Взаимодействие с башней |
| `FsmWave` | `Timer`, `Begin`, `Go`, `End`, `Wait` | Состояние волны мобов |
| `FsmSkill` | `None`, `Begin`, `SetTarget`, `ShowEffect`, `End` | Применение скилла |
| `FsmWarrior` | `New`, `Await`, `GoToMob`, `Attack`, `GoToRepair`, `Repair`, `GoToPlacement`, `Dead` | Состояние воина |

## Диаграммы состояний

### FsmWave (цикл волны)

```
Timer ──(таймер истёк)──→ Begin ──(пауза)──→ Go ──(все вышли)──→ End
  ↑                                                                  │
  │                                    ┌─────────────────────────────┤
  │                                    │                             │
  │                              (не все убиты)               (все убиты)
  │                                    │                             │
  │                              (дорога занята)              (дорога свободна)
  │                                    ↓                             ↓
  └──(дорога свободна)── Wait ←───    └──────── Возврат в Timer ───→(если есть волны)
```

1. **FsmStateWaveTimer** — обратный отсчёт времени до следующей волны
2. **FsmStateWaveBegin** — увеличивает CurrentWave, создаёт мобов в буфер
3. **FsmStateWaveGo** — выводит мобов из буфера на дорогу с интервалом
4. **FsmStateWaveEnd** — все мобы вышли на дорогу
5. **FsmStateWaveWait** — ожидание уничтожения всех мобов

### FsmGameplay (общее состояние игры)

```
GamePlay ←──→ BuildBegin ←──→ Build ←──→ BuildEnd
   │  ↑
   │  └──────────────┐
   ↓                 │
GamePause            │
   │                 │
   ↓                 │
SelectSkill ←──→ SetSkill
```

### FsmTower (взаимодействие с башней)

```
None ←──→ Selected ←──→ Delete
   │          │
   │          └──→ Placement ←──→ PlacementEnd
   └──────────────────────────────────┘
```

## Пример использования

```csharp
// Создание автомата
var fsmGameplay = new FsmGameplay(container);

// Установка состояния
fsmGameplay.Fsm.SetState<FsmStateBuildBegin>();

// Подписка на смену состояния
fsmGameplay.Fsm.StateCurrent.Subscribe(newState => {
    if (newState is FsmStateGamePlay)
    {
        // Игра активна — запустить таймеры
    }
    if (newState is FsmStateGamePause)
    {
        // Пауза — Time.timeScale = 0
    }
});

// Проверка текущего состояния
if (fsmGameplay.IsStateBuilding())
{
    // Разрешено строительство
}
```

## Связь FSM с UI и сервисами

Панели UI показываются/скрываются автоматически по FSM:

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

Сервисы подписываются на FSM для управления логикой:

```csharp
// WaveService подписывается на FsmWave
_fsmWave.Fsm.StateCurrent.Subscribe(state => {
    if (state is FsmStateWaveTimer) StartTimer();
    if (state is FsmStateWaveBegin) GenerateMobsToBuffer();
    if (state is FsmStateWaveGo) StartMobGeneration();
    if (state is FsmStateWaveWait) CheckAllMobsDead();
});
```

---

*Читайте далее: [06_MVVM.md](06_MVVM.md)*
