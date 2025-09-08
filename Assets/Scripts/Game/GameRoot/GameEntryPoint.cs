using System.Collections;
using DI;
using Game.Common;
using Game.GamePlay.Root;
using Game.GameRoot.Commands;
using Game.GameRoot.Commands.HardCurrency;
using Game.GameRoot.ImageManager;
using Game.GameRoot.Services;
using Game.MainMenu.Root;
using Game.Settings;
using Game.State;
using MVVM.CMD;
using Newtonsoft.Json;
using R3;
using Scripts.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            /**
             * Системные настройки
             * FPS и др.
             */
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep; //Не гаснуть экран


            _instance = new GameEntryPoint();

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

            //Настройки приложения
            var settingsProvider = new SettingsProvider();
            _rootContainer.RegisterInstance<ISettingsProvider>(settingsProvider);


            var gameStateProvider = new PlayerPrefsGameStateProvider(); //Заменить конструктор на другой - из облака
            gameStateProvider.LoadSettingsState(); //Загрузили настройки игры
            //Применяем настройки к окружению - звук, вибрация и т.п.

            gameStateProvider.LoadGameState(); //Загружаем данные игрока.

            var cmd = new CommandProcessor(gameStateProvider); //Создаем обработчик команд
            _rootContainer.RegisterInstance<ICommandProcessor>(cmd); //Кешируем его в DI


            //Регистрируем общие команды для всей игры.
            //Потратить валюту.
            // Debug.Log(JsonConvert.SerializeObject());
            cmd.RegisterHandler(new CommandSpendHardCurrencyHandler(gameStateProvider.GameState));
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

        private async void RunGame()
        {
            await _rootContainer.Resolve<ISettingsProvider>().LoadGameSettings();

            /*
#if UNITY_EDITOR
            var sceneName = SceneManager.GetActiveScene().name;
            if (sceneName == Scenes.GAMEPLAY)
            {
                var enterParams = new GameplayEnterParams(0);
              //  enterParams.HasSessionGameplay = true;
                _coroutines.StartCoroutine(LoadAndStartGameplay(enterParams));
                return;
            }

         /*   if (sceneName == Scenes.MAINMENU)
            {
                _coroutines.StartCoroutine(LoadAndStartMainMenu());
                return;
            }
            if (sceneName != Scenes.BOOT)
            {
                return;
            }*/
            /*
 #endif*/
            _coroutines.StartCoroutine(LoadAndStartMainMenu());
        }

        private IEnumerator LoadAndStartGameplay(GameplayEnterParams enterParams)
        {
            _uiRoot.ShowLoadingScreen();
            _cachedSceneContainer?.Dispose();
            yield return LoadScene(Scenes.BOOT);
            Debug.Log("0");
            yield return LoadScene(Scenes.GAMEPLAY);

            //  yield return new WaitForSeconds(1);
            //Ждем когда загрузится сохранение игры
            //var isGameStateLoaded = false; //не загружено
            //При загрузке, по подписке поменяем флажок на Загружено

            Debug.Log("1");
            _rootContainer.Resolve<IGameStateProvider>().LoadGameplayState();
                //.Subscribe(_ => isGameStateLoaded = true);

           // yield return new WaitUntil(() => isGameStateLoaded);
            Debug.Log("3");
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

        private IEnumerator LoadAndStartMainMenu(MainMenuEnterParams enterParams = null)
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

        private IEnumerator LoadFirstBoot()
        {
            yield return LoadScene(Scenes.FIRST_BOOT);
            yield return new WaitForSeconds(1);
            _instance.RunGame();
        }

        private IEnumerator LoadScene(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
        }
    }
}