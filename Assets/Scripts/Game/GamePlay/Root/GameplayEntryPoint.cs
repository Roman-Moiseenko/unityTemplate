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
            Debug.Log("GameplayEntryPoint");
            var exitSceneRequest = new Subject<GameplayExitParams>();
            gameplayContainer.RegisterInstance(exitSceneRequest);
            //StartCoroutine(Initialisation(gameplayContainer, enterParams));
            //Регистрируем событие движение камеры
            var positionGameplayCamera = new Subject<Unit>();
            gameplayContainer.RegisterInstance(AppConstants.CAMERA_MOVING, positionGameplayCamera);
           // var hardCurrencyService = new HardCurrencyService();
            //gameplayContainer.RegisterInstance(hardCurrencyService);
            gameplayContainer.RegisterInstance(enterParams); //На всякий случай сохраняем в контейнере входные данные
            
            GameplayRegistrations.Register(gameplayContainer, enterParams); //Регистрируем все сервисы сцены
            var gameplayViewModelsContainer = new DIContainer(gameplayContainer); //Создаем контейнер для view-моделей
            GameplayViewModelsRegistrations.Register(gameplayViewModelsContainer); //Регистрируем все View-модели сцены Gameplay
            InitWorld(gameplayViewModelsContainer);
            InitUI(gameplayViewModelsContainer);
            
            //Сохраняем начальные параметры игровой сессии
            gameplayContainer.Resolve<IGameStateProvider>().SaveGameplayState();
            
            Debug.Log($"GAME PLAY ENTER POINT: Results MapId {enterParams?.MapId}");
            
            gameplayContainer.Resolve<WaveService>().Start(); //Запуск таймера
            
            //Создаем выходные параметры для входа в Меню
         //   var mainMenuEnterParams = new MainMenuEnterParams("Fatality");
           // var exitParams = new GameplayExitParams(mainMenuEnterParams);
            //Формируем сигнал для подписки
          //  var exitSceneRequest = gameplayContainer.Resolve<Subject<GameplayExitParams>>();
           // var exitToMainMenuSignal = exitSceneRequest.Select(_ => exitParams);//В сигнал кладем в выходные параметры
            return exitSceneRequest;
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