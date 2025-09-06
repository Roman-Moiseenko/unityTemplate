using System.Collections;
using DI;
using Game.Common;
using Game.GamePlay.Classes;
using Game.GamePlay.Root.View;
using Game.GamePlay.Services;
using Game.GamePlay.View.UI;
using Game.GamePlay.View.Waves;
using Game.GameRoot.Services;
using Game.MainMenu.Root;
using Game.State;
using Newtonsoft.Json;
using R3;
using Scripts.Game.GameRoot;
using UnityEngine;

namespace Game.GamePlay.Root
{
    public class GameplayEntryPoint : MonoBehaviour
    {
        //Преваб интерфейса UI текущей сцены 
        [SerializeField] private UIGameplayRootBinder _sceneUIRootPrefab; 

        //Объект сцены, куда будут вставляться/создаваться объекты игры из префабов
        [SerializeField] private WorldGameplayRootBinder _worldRootBinder;
        
        public Observable<GameplayExitParams> Run(DIContainer gameplayContainer, GameplayEnterParams enterParams)
        {
            var exitSceneRequest = new Subject<GameplayExitParams>();
            gameplayContainer.RegisterInstance(exitSceneRequest);
            //StartCoroutine(Initialisation(gameplayContainer, enterParams));
            Debug.Log("1 " + Time.deltaTime);
            //Регистрируем событие движение камеры
            //1
            var positionGameplayCamera = new Subject<Unit>();
            gameplayContainer.RegisterInstance(AppConstants.CAMERA_MOVING, positionGameplayCamera);
            Debug.Log("2 " + Time.deltaTime);
            //2
           // var hardCurrencyService = new HardCurrencyService();
           // Debug.Log("2.5 " + Time.deltaTime);
            //gameplayContainer.RegisterInstance(hardCurrencyService);
            Debug.Log("3 " + Time.deltaTime);
            //3
            GameplayRegistrations.Register(gameplayContainer, enterParams); //Регистрируем все сервисы сцены
            Debug.Log("4 " + Time.deltaTime);
            //4
            var gameplayViewModelsContainer = new DIContainer(gameplayContainer); //Создаем контейнер для view-моделей
            GameplayViewModelsRegistrations.Register(gameplayViewModelsContainer); //Регистрируем все View-модели сцены Gameplay
            Debug.Log("5 " + Time.deltaTime);
            //5
            InitWorld(gameplayViewModelsContainer);
            //6
            Debug.Log("6 " + Time.deltaTime);
            InitUI(gameplayViewModelsContainer);
            
            //Сохраняем начальные параметры игровой сессии
            //7
            Debug.Log("7 " + Time.deltaTime);
            gameplayContainer.Resolve<IGameStateProvider>().SaveGameplayState();
            
            Debug.Log($"GAME PLAY ENTER POINT: Results MapId {enterParams?.MapId}");

            //Создаем выходные параметры для входа в Меню
         //   var mainMenuEnterParams = new MainMenuEnterParams("Fatality");
           // var exitParams = new GameplayExitParams(mainMenuEnterParams);
            //Формируем сигнал для подписки
          //  var exitSceneRequest = gameplayContainer.Resolve<Subject<GameplayExitParams>>();
           // var exitToMainMenuSignal = exitSceneRequest.Select(_ => exitParams);//В сигнал кладем в выходные параметры
            return exitSceneRequest;
        }

        public IEnumerator Initialisation(DIContainer gameplayContainer, GameplayEnterParams enterParams)
        {
            Debug.Log($"time 0 = {Time.deltaTime}");
            var positionGameplayCamera = new Subject<Unit>();
            gameplayContainer.RegisterInstance(AppConstants.CAMERA_MOVING, positionGameplayCamera);
            Debug.Log($"time 1 = {Time.deltaTime}");
            yield return null;
            var hardCurrencyService = new HardCurrencyService();
            gameplayContainer.RegisterInstance(hardCurrencyService);
            Debug.Log($"time 2 = {Time.deltaTime}");
            yield return null;
            GameplayRegistrations.Register(gameplayContainer, enterParams);
            Debug.Log($"time 3 = {Time.deltaTime}");
            yield return null;
            var gameplayViewModelsContainer = new DIContainer(gameplayContainer); //Создаем контейнер для view-моделей
            GameplayViewModelsRegistrations.Register(gameplayViewModelsContainer); //Регистрируем все View-модели сцены Gameplay
            Debug.Log($"time 4 = {Time.deltaTime}");
            yield return null;
            InitWorld(gameplayViewModelsContainer);
            Debug.Log($"time 5 = {Time.deltaTime}");
            yield return null;            
            InitUI(gameplayViewModelsContainer);
            Debug.Log($"time 6 = {Time.deltaTime}");
            yield return null;           
            gameplayContainer.Resolve<IGameStateProvider>().SaveGameplayState();
            Debug.Log($"time 7 = {Time.deltaTime}");
            yield return null;   
        }
        
        private void InitWorld(DIContainer viewContainer)
        {
            //Строим мир сцены по параметрам
            _worldRootBinder.Bind(viewContainer.Resolve<WorldGameplayRootViewModel>());
        }

        private void InitUI(DIContainer viewContainer)
        {
            var uiRoot = viewContainer.Resolve<UIRootView>(); //Находим рутовый контейнер с UI и присоединем загруженный UI
            var uiSceneRootBinder = Instantiate(_sceneUIRootPrefab); //Загружаем UI из префаба
            uiRoot.AttachSceneUI(uiSceneRootBinder.gameObject);

            var uiSceneRootViewModel = viewContainer.Resolve<UIGameplayRootViewModel>();
            uiSceneRootBinder.Bind(uiSceneRootViewModel);
            
            //можно открывать окошки
            var uiManager = viewContainer.Resolve<GameplayUIManager>();
            uiManager.OpenScreenGameplay();
        }
    }
}