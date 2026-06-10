# 2. Структура проекта (директории)

## Полная структура

```
Assets/Scripts/
│
├── DI/                                    # Dependency Injection контейнер
│   ├── DIContainer.cs                     #   Основной контейнер (родительский scoping)
│   └── DIEntry.cs                         #   Entry-класс для фабрик и singleton
│
├── Game/
│   │
│   ├── GamePlay/                          # Всё, что относится к геймплею
│   │   ├── Root/                          #   Точка входа в сцену Gameplay
│   │   │   ├── GameplayEntryPoint.cs      #     MonoBehaviour-точка входа
│   │   │   ├── GameplayRegistrations.cs   #     Регистрация сервисов, FSM, CQRS
│   │   │   ├── GameplayEnterParams.cs     #     Входные параметры (MapId, TypeGameplay, колоды)
│   │   │   ├── GameplayExitParams.cs      #     Выходные параметры (MainMenuEnterParams)
│   │   │   └── View/                      #     Регистрация ViewModel сцены
│   │   │       └── GameplayViewModelsRegistrations.cs
│   │   ├── Fsm/                           #   Конечные автоматы
│   │   │   ├── FsmGameplay.cs             #     Общее состояние геймплея
│   │   │   ├── FsmTower.cs                #     Состояние взаимодействия с башней
│   │   │   ├── FsmWave.cs                 #     Состояние волны мобов
│   │   │   ├── FsmSkill.cs                #     Состояние применения скилла
│   │   │   └── GameplayStates/            #     Конкретные состояния
│   │   ├── Services/                      #   Игровые сервисы
│   │   │   ├── WaveService.cs             #     Управление волнами мобов
│   │   │   ├── TowersService.cs           #     Управление башнями
│   │   │   ├── SkillsService.cs           #     Управление скиллами
│   │   │   ├── CastleService.cs           #     Управление замком
│   │   │   ├── HeroService.cs             #     Управление героем
│   │   │   ├── WayService.cs              #     Расчёт пути для мобов
│   │   │   ├── RoadsService.cs            #     Управление дорогами
│   │   │   ├── GroundsService.cs          #     Управление землёй (клетками)
│   │   │   ├── PlacementService.cs        #     Проверка размещения объектов
│   │   │   ├── WarriorService.cs          #     Управление воинами
│   │   │   ├── DamageService.cs           #     Расчёт урона
│   │   │   ├── RewardProgressService.cs   #     Прогресс и награды
│   │   │   ├── GameplayService.cs         #     Отслеживание конца игры
│   │   │   ├── InputController.cs         #     Обработка ввода
│   │   │   ├── FrameService.cs            #     Визуальный фрейм башен
│   │   │   ├── FrameSkillService.cs       #     Визуальный фрейм скиллов
│   │   │   └── FramePlacementService.cs   #     Фрейм размещения башни
│   │   ├── Commands/                      #   CQRS команды
│   │   │   ├── CastleCommands/            #     Команды замка
│   │   │   ├── GroundCommands/            #     Команды земли
│   │   │   ├── MapCommand/                #     Команды карты
│   │   │   ├── MobCommands/               #     Команды мобов
│   │   │   ├── RewardCommand/             #     Команды наград
│   │   │   ├── RoadCommand/               #     Команды дорог
│   │   │   ├── TowerCommand/              #     Команды башен
│   │   │   ├── WarriorCommands/           #     Команды воинов
│   │   │   └── WaveCommands/              #     Команды волн
│   │   ├── Queries/                       #   CQRS запросы
│   │   │   ├── Classes/                   #     Классы запросов
│   │   │   ├── SkillQueries/              #     Запросы скиллов
│   │   │   ├── TowerQueries/              #     Запросы башен
│   │   │   └── WaveQueries/               #     Запросы волн
│   │   ├── Classes/                       #   Вспомогательные классы
│   │   └── View/                          #   MVVM (ViewModels + Binders)
│   │       ├── Mobs/                      #     ViewModel мобов
│   │       ├── Towers/                    #     ViewModel башен
│   │       ├── UI/                        #     UI-элементы (панели, попапы)
│   │       └── ...                        #     Остальные View
│   │
│   ├── GameRoot/                          # Точка входа в игру
│   │   ├── GameEntryPoint.cs              #   Старт игры (автостарт)
│   │   ├── Scenes.cs                      #   Список сцен (Boot, MainMenu, Gameplay)
│   │   └── Services/                      #   Глобальные сервисы
│   │       ├── AdService.cs               #     Работа с рекламой
│   │       ├── ResourceService.cs         #     Ресурсы (подписки, бустеры)
│   │       └── GenerateService.cs         #     Генерация данных
│   │
│   ├── MainMenu/                          # Главное меню
│   │   ├── Root/                          #   Точка входа
│   │   │   ├── MainMenuEntryPoint.cs      #     Точка входа
│   │   │   ├── MainMenuRegistrations.cs   #     Регистрация сервисов
│   │   │   ├── MainMenuEnterParams.cs     #     Входные параметры (награды, результаты)
│   │   │   ├── MainMenuExitParams.cs      #     Выходные параметры (выбор карты)
│   │   │   └── View/                      #     Регистрация ViewModel
│   │   ├── Services/                      #   Сервисы меню
│   │   ├── Commands/                      #   Команды меню
│   │   └── View/                          #   UI элементы меню
│   │
│   ├── Settings/                          # Настройки игры
│   │   ├── GameSettings.cs                #   Все настройки (карты, башни, скиллы, враги)
│   │   ├── ISettingsProvider.cs           #   Интерфейс провайдера
│   │   ├── ApplicationSettings.cs         #   Настройки приложения
│   │   └── Gameplay/                      #   Детальные настройки
│   │       ├── Entities/ (Tower, Skill, Hero, Castle)
│   │       ├── Maps/                      #     Настройки карт
│   │       ├── Enemies/                   #     Настройки врагов
│   │       └── Initial/                   #     Начальные настройки (замок, инвентарь)
│   │
│   ├── State/                             # Состояние игры
│   │   ├── IGameStateProvider.cs          #   Интерфейс провайдера
│   │   ├── WebGameStateProvider.cs        #   Реализация (веб + PlayerPrefs)
│   │   ├── Root/                          #   GameState
│   │   │   ├── GameState.cs               #     POCO-данные игрока
│   │   │   ├── GameStateProxy.cs          #     Реактивная обёртка
│   │   │   ├── GameSettingsStateProxy.cs  #     Реактивная обёртка настроек
│   │   │   └── DefaultGameState.cs        #     Дефолтное состояние
│   │   ├── Gameplay/                      #   GameplayState
│   │   │   ├── GameplayState.cs           #     POCO-данные сессии
│   │   │   └── GameplayStateProxy.cs      #     Реактивная обёртка
│   │   ├── Maps/                          #   Сущности карты
│   │   │   ├── Towers/ (TowerEntity, TowerEntityData)
│   │   │   ├── Mobs/ (MobEntity)
│   │   │   ├── Roads/ (RoadEntity)
│   │   │   ├── Grounds/ (GroundEntity)
│   │   │   ├── Castle/ (CastleEntity)
│   │   │   ├── Shots/ (ShotData)
│   │   │   ├── Skills/ (SkillEntity)
│   │   │   ├── Warriors/ (WarriorEntity)
│   │   │   ├── Heroes/ (HeroEntity)
│   │   │   └── Rewards/ (RewardEntityData)
│   │   └── Inventory/                     #   Инвентарь
│   │       ├── HeroCards/
│   │       ├── TowerCards/
│   │       ├── SkillCards/
│   │       ├── Chests/
│   │       └── Common/
│   │
│   └── Common/                            # Общие константы и утилиты
│       └── AppConstants.cs                #   Константы
│
├── MVVM/                                  # MVVM фреймворк
│   ├── UI/                                #   Базовые UI-компоненты
│   │   ├── UIRootViewModel.cs             #     Корневая ViewModel
│   │   ├── UIRootBinder.cs                #     Корневой Binder
│   │   ├── PanelBinder.cs                #     Базовый класс панелей
│   │   ├── PopupBinder.cs                #     Базовый класс попапов
│   │   └── UIManager.cs                   #     Менеджер UI
│   ├── CMD/                               #   CQRS
│   │   ├── ICommand.cs                    #     Интерфейс команд
│   │   ├── IQuery.cs                      #     Интерфейс запросов
│   │   ├── CommandProcessor.cs            #     Процессор команд
│   │   └── QueryProcessor.cs              #     Процессор запросов
│   ├── FSM/                               #   Конечные автоматы
│   │   ├── FSM.cs                         #     Базовый FSM
│   │   └── FsmProxy.cs                    #     Реактивная обёртка
│   └── Storage/                           #   Хранилища
│       ├── PoolMono.cs                    #     Object Pooling
│       └── StorageManager.cs              #     Кэш текстур
│
├── Utils/                                 # Вспомогательные утилиты
│   ├── LoadingState.cs                    #   Состояние загрузки
│   └── Coroutines.cs                      #   Корутины
│
└── Localization/                          # Локализация
    ├── LocalizationManager.cs
    ├── ILocalizationProvider.cs
    └── LocalizationText.cs
```

---

*Читайте далее: [03_DI_Container.md](03_DI_Container.md)*
