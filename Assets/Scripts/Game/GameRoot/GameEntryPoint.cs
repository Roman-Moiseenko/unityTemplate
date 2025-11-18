using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using DI;
using Game.Common;
using Game.GamePlay.Root;
using Game.GameRoot.Commands;
using Game.GameRoot.Commands.HardCurrency;
using Game.GameRoot.ImageManager;
using Game.GameRoot.Services;
using Game.MainMenu.Root;
using Game.Settings;
using Game.Settings.Gameplay.Maps;
using Game.State;
using Game.State.Inventory;
using Game.State.Inventory.Chests;
using Game.State.Root;
using MVVM.CMD;
using Newtonsoft.Json;
using R3;
using Scripts.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Scripts.Game.GameRoot
{
    public class GameEntryPoint
    {
        private static GameEntryPoint _instance;
        private Coroutines _coroutines;
        private UIRootView _uiRoot;
        private readonly DIContainer _rootContainer = new();
        private DIContainer _cachedSceneContainer;
        private ImageManagerBinder _imageManager;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void AutostartGame()
        {
/*
            var d = new MapRewardSetting();
            var l = new List<RewardItem>();
            l.Add(new RewardItem
            {
                ConfigId = "Tower01",
                Type = InventoryType.TowerCard,
                Amount = 1,
            });
            l.Add(new RewardItem
            {
                ConfigId = "",
                Type = InventoryType.SoftCurrency,
                Amount = 9999,
            });
            //d.RewardOnWave.Add(10, l);
            d.RewardChest.Add(TypeChest.Legend, l);
            Debug.Log(JsonConvert.SerializeObject(d, Formatting.Indented));
            */
            /**
             * Системные настройки
             * FPS и др.
             */
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep; //Не гаснуть экран


            _instance = new GameEntryPoint();

            // _instance.LoadState();
            _instance.RunGame();
        }

        private GameEntryPoint()
        {
            //Возможность запуска корутины из не монобехевиара
            _coroutines = new GameObject("[COROUTINES]").AddComponent<Coroutines>();
            Object.DontDestroyOnLoad(_coroutines.gameObject);
            //  _coroutines.StartCoroutine(LoadFirstBoot());

            //Находим прехаб UIRoot и присоединяем его к проекту
            var prefabUIRoot = Resources.Load<UIRootView>("UIRoot");
            _uiRoot = Object.Instantiate(prefabUIRoot);
            Object.DontDestroyOnLoad(_uiRoot.gameObject);
            _rootContainer.RegisterInstance(_uiRoot); //Регистрируем в корневом контейнере

            //Находим менеджер изображений
            var prefabImageManager = Resources.Load<ImageManagerBinder>("ImageManager");
            _imageManager = Object.Instantiate(prefabImageManager);
            _imageManager.gameObject.name = AppConstants.IMAGE_MANAGER;
            Object.DontDestroyOnLoad(_imageManager.gameObject);


            /*
                string userId;
                string userToken;
                var webService = new WebService();
                //Проверяем игрока первый запуск?
                if (!PlayerPrefs.HasKey(AppConstants.USER_ID))
                {
                    userId = Path.GetRandomFileName();
                    PlayerPrefs.SetString(AppConstants.USER_ID, userId);
                    webService.FirstAuthorization(userId);
                    //TODO Первичная авторизация клиента
                }
                else
                {
                    userId = PlayerPrefs.GetString(AppConstants.USER_ID);
                    userToken = PlayerPrefs.GetString(AppConstants.USER_TOKEN);
                }
                */
            //
            //var gameStateProvider = new PlayerPrefsGameStateProvider(); //Заменить конструктор на другой - из облака
            var gameStateProvider = new WebGameStateProvider();
            // gameStateProvider.LoadSettingsState(); //Загрузили настройки игры  

            //Настройки приложения
            var settingsProvider = new SettingsProviderWeb();
            _rootContainer.RegisterInstance<ISettingsProvider>(settingsProvider);


            // gameStateProvider.LoadGameState(); //Загружаем данные игрока.

            var cmd = new CommandProcessor(gameStateProvider); //Создаем обработчик команд
            _rootContainer.RegisterInstance<ICommandProcessor>(cmd); //Кешируем его в DI


            //Регистрируем общие команды для всей игры.
            //Потратить валюту.
            // Debug.Log(JsonConvert.SerializeObject());
//            cmd.RegisterHandler(new CommandSpendHardCurrencyHandler(gameStateProvider.GameState));
            cmd.RegisterHandler(new CommandSaveGameStateHandler());
            _rootContainer.RegisterInstance<IGameStateProvider>(gameStateProvider);
            //_rootContainer.RegisterFactory(c => new SomeCommonService()).AsSingle(); //Сервис ... создастся при первом вызове
            _rootContainer.RegisterFactory(c => new AdService(_rootContainer)).AsSingle(); //Сервис рекламы
            _rootContainer.RegisterFactory(c => new ResourceService(_rootContainer)).AsSingle(); //Сервис ресурсов

            //Положить в контейнер настройки игры ....
            //Сервисы аналитики, платежки, 

            var gen = new GenerateService();
            _rootContainer.RegisterFactory(c => gen);
        }

        private void RegisterService()
        {
        }

        private void RunGame() //Нестатичный вызов.
        {
            _coroutines.StartCoroutine(LoadFirstBoot());
        }

        private IEnumerator LoadFirstBoot()
        {
            _uiRoot.ShowLoadingFirstScreen();
            _cachedSceneContainer?.Dispose();
            yield return LoadScene(Scenes.BOOT);
            yield return LoadScene(Scenes.MAINMENU);
            yield return new WaitForSeconds(1);

            //Загружаем пользователя
            var provider = _rootContainer.Resolve<IGameStateProvider>();

            var loadedSettings = new LoadingState();

            provider.CheckWebAvailable().Subscribe(v => loadedSettings = v);
            while (!loadedSettings.Loaded)
            {
                _uiRoot.TextLoadingFirst(loadedSettings.TextState.CurrentValue);
                yield return null;
            }
            //   Debug.Log("Check Available");

            provider.LoadSettingsState().Subscribe(v => loadedSettings = v);
            while (!loadedSettings.Loaded)
            {
                _uiRoot.TextLoadingFirst(loadedSettings.TextState.CurrentValue);
                yield return null;
            }
            //   Debug.Log("Settings Load");

            loadedSettings.Clear();
            provider.LoadGameState().Subscribe(v => loadedSettings = v);
            while (!loadedSettings.Loaded)
            {
                _uiRoot.TextLoadingFirst(loadedSettings.TextState.CurrentValue);
                yield return null;
            }

            //   Debug.Log("GameState Load");
            //Загружаем данные по игре
            var settings = _rootContainer.Resolve<ISettingsProvider>();
            loadedSettings.Clear();
            settings.LoadGameSettings().Subscribe(v => loadedSettings = v);
            while (!loadedSettings.Loaded)
            {
                _uiRoot.TextLoadingFirst(loadedSettings.TextState.CurrentValue);
                yield return null;
            }
            //Загружаем новые ресурсы ...

            //Регистрируем общие команды и сервисы, зависимые от gameStateProvider
            _rootContainer.Resolve<ICommandProcessor>()
                .RegisterHandler(new CommandSpendHardCurrencyHandler(provider.GameState));
            _rootContainer.Resolve<ICommandProcessor>()
                .RegisterHandler(new CommandAddHardCurrencyHandler(provider.GameState));

            //Применяем настройки пользователя к игре
            _uiRoot.TextLoadingFirst("Регистрируем настройки");
            yield return null;
            //Контейнер
            var sceneEntryPoint = Object.FindFirstObjectByType<MainMenuEntryPoint>();
            var mainMenuContainer = _cachedSceneContainer = new DIContainer(_rootContainer);
            sceneEntryPoint.Run(mainMenuContainer, null).Subscribe(mainMenuExitParams =>
            {
                var targetSceneName = mainMenuExitParams.TargetSceneEnterParams.SceneName;
                if (targetSceneName == Scenes.GAMEPLAY)
                {
                    _coroutines.StartCoroutine(
                        LoadAndStartGameplay(mainMenuExitParams.TargetSceneEnterParams.As<GameplayEnterParams>())
                    );
                }
            });
            _uiRoot.HideLoadingFirstScreen();
        }

        private IEnumerator LoadAndStartGameplay(GameplayEnterParams enterParams)
        {
            _uiRoot.ShowLoadingScreen();
            _cachedSceneContainer?.Dispose();
            yield return LoadScene(Scenes.BOOT);
            //   Debug.Log("0");
            yield return LoadScene(Scenes.GAMEPLAY);

            //  yield return new WaitForSeconds(1);
            //Ждем когда загрузится сохранение игры
            //var isGameStateLoaded = false; //не загружено
            //При загрузке, по подписке поменяем флажок на Загружено

            // Debug.Log("1");
            _rootContainer.Resolve<IGameStateProvider>().LoadGameplayState();
            //.Subscribe(_ => isGameStateLoaded = true);

            // yield return new WaitUntil(() => isGameStateLoaded);
            //    Debug.Log("3");
            //Контейнер
            var sceneEntryPoint = Object.FindFirstObjectByType<GameplayEntryPoint>();
            var gameplayContainer = _cachedSceneContainer = new DIContainer(_rootContainer);

            sceneEntryPoint.Run(gameplayContainer, enterParams).Subscribe(gameplayExitParams =>
            {
                {
                    //Debug.Log("enterParams = " + JsonConvert.SerializeObject(gameplayExitParams, Formatting.Indented));
                    _coroutines.StartCoroutine(LoadAndStartMainMenu(gameplayExitParams.MainMenuEnterParams));
                    if (gameplayExitParams.SaveGameplay == false)
                        _rootContainer.Resolve<IGameStateProvider>()
                            .ResetGameplayState(); //При выходе сбрасываем данные
                }
            });

            _uiRoot.HideLoadingScreen();
        }

        private IEnumerator LoadAndStartMainMenu(MainMenuEnterParams enterParams)
        {
            _uiRoot.ShowLoadingScreen();
            _cachedSceneContainer?.Dispose();
            yield return LoadScene(Scenes.BOOT);
            yield return LoadScene(Scenes.MAINMENU);


            //   yield return new WaitForSeconds(1);

            //Контейнер
            var sceneEntryPoint = Object.FindFirstObjectByType<MainMenuEntryPoint>();
            var mainMenuContainer = _cachedSceneContainer = new DIContainer(_rootContainer);
            sceneEntryPoint.Run(mainMenuContainer, enterParams).Subscribe(mainMenuExitParams =>
            {
                var targetSceneName = mainMenuExitParams.TargetSceneEnterParams.SceneName;
                if (targetSceneName == Scenes.GAMEPLAY)
                {
                    _coroutines.StartCoroutine(
                        LoadAndStartGameplay(mainMenuExitParams.TargetSceneEnterParams.As<GameplayEnterParams>())
                    );
                }
            });

            _uiRoot.HideLoadingScreen();
        }


        private IEnumerator LoadScene(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
        }
    }
}