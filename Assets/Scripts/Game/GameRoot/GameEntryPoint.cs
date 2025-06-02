using System.Collections;
using DI;
using Game.GamePlay.Root;
using Game.MainMenu.Root;
using Game.Settings;
using Game.State;
using Game.State.CMD;
using R3;
using Scripts.Game.GameRoot.Services;
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
            _coroutines = new GameObject("[COROUTINES]").AddComponent<Coroutines>();
            Object.DontDestroyOnLoad(_coroutines.gameObject);
          //  _coroutines.StartCoroutine(LoadFirstBoot());
            
            //Находим прехаб UIRoot и присоединяем его к проекту
            var prefabUIRoot = Resources.Load<UIRootView>("UIRoot");
            _uiRoot = Object.Instantiate(prefabUIRoot);
            Object.DontDestroyOnLoad(_uiRoot.gameObject);
            _rootContainer.RegisterInstance(_uiRoot); //Регистрируем в корневом контейнере
            
            //Настройки приложения
            var settingsProvider = new SettingsProvider();
            _rootContainer.RegisterInstance<ISettingsProvider>(settingsProvider);
            

            var gameStateProvider = new PlayerPrefsGameStateProvider(); //Заменить конструктор на другой - из облака
            gameStateProvider.LoadSettingsState(); //Загрузили настройки игры
            //Применяем настройки к окружению - звук, вибрация и т.п.
            
            _rootContainer.RegisterInstance<IGameStateProvider>(gameStateProvider);
            
            _rootContainer.RegisterFactory(c => new SomeCommonService()).AsSingle(); //Сервис ... создастся при первом вызове
            
            var cmd = new CommandProcessor(gameStateProvider); //Создаем обработчик команд
            _rootContainer.RegisterInstance<ICommandProcessor>(cmd); //Кешируем его в DI
            
            //Положить в контейнер настройки игры ....
            //Сервисы аналитики, платежки, 
        }
        
        private async void RunGame()
        {
            await _rootContainer.Resolve<ISettingsProvider>().LoadGameSettings();
            
#if UNITY_EDITOR
            var sceneName = SceneManager.GetActiveScene().name;
            if (sceneName == Scenes.GAMEPLAY)
            {
                var enterParams = new GameplayEnterParams(0);
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
            
#endif
            _coroutines.StartCoroutine(LoadAndStartMainMenu());
        }

        private IEnumerator LoadAndStartGameplay(GameplayEnterParams enterParams)
        {
            _uiRoot.ShowLoadingScreen();
            _cachedSceneContainer?.Dispose();
            yield return LoadScene(Scenes.BOOT);
            yield return LoadScene(Scenes.GAMEPLAY);

            yield return new WaitForSeconds(1);
            //Ждем когда загрузится сохранение игры
            var isGameStateLoaded = false; //не загружено
            //При загрузке, по подписке поменяем флажок на Загружено
            _rootContainer.Resolve<IGameStateProvider>().LoadGameState().Subscribe(_ => isGameStateLoaded = true);
            yield return new WaitUntil(() => isGameStateLoaded);
 
            //Контейнер
            var sceneEntryPoint = Object.FindFirstObjectByType<GameplayEntryPoint>();
            var gameplayContainer = _cachedSceneContainer = new DIContainer(_rootContainer);
            sceneEntryPoint.Run(gameplayContainer, enterParams).Subscribe(gameplayExitParams =>
            {
                _coroutines.StartCoroutine(LoadAndStartMainMenu(gameplayExitParams.MainMenuEnterParams));
            });
            

            _uiRoot.HideLoadingScreen();
        }

        private IEnumerator LoadAndStartMainMenu(MainMenuEnterParams enterParams = null)
        {
            _uiRoot.ShowLoadingScreen();
            _cachedSceneContainer?.Dispose();
            yield return LoadScene(Scenes.BOOT);
            yield return LoadScene(Scenes.MAINMENU);
            
            yield return new WaitForSeconds(1);
            
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

