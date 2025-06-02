using DI;
using Game.Common;
using Game.GamePlay.Commands;
using Game.GamePlay.Root.View;
using Game.GamePlay.Services;
using Game.GamePlay.View.UI;
using Game.MainMenu.Root;
using Game.State;
using Game.State.CMD;
using ObservableCollections;
using R3;
using Scripts.Game.GameRoot;
using UnityEngine;
using Random = UnityEngine.Random;

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
            GameplayRegistrations.Register(gameplayContainer, enterParams); //Регистрируем все сервисы сцены
            var gameplayViewModelsContainer = new DIContainer(gameplayContainer); //Создаем контейнер для view-моделей
            GameplayViewModelsRegistrations.Register(gameplayViewModelsContainer); //Регистрируем все View-модели сцены Gameplay
            
            InitWorld(gameplayViewModelsContainer);
            InitUI(gameplayViewModelsContainer);
            
            Debug.Log($"MAIN MENU ENTER POINT: Results MapId {enterParams?.MapId}");

                //Создаем выходные параметры для входа в Меню
            var mainMenuEnterParams = new MainMenuEnterParams("Fatality");
            var exitParams = new GameplayExitParams(mainMenuEnterParams);
            //Формируем сигнал для подписки
            var exitSceneRequest = gameplayContainer.Resolve<Subject<Unit>>(AppConstants.EXIT_SCENE_REQUEST_TAG);
            var exitToMainMenuSignal = exitSceneRequest.Select(_ => exitParams);//В сигнал кладем в выходные параметры
            return exitToMainMenuSignal;
        }

        private void InitWorld(DIContainer viewsDiContainer)
        {
            _worldRootBinder.Bind(viewsDiContainer.Resolve<WorldGameplayRootViewModel>());
        }

        private void InitUI(DIContainer viewsDiContainer)
        {
            var uiRoot = viewsDiContainer.Resolve<UIRootView>(); //Находим рутовый контейнер с UI и присоединем загруженный UI
            var uiSceneRootBinder = Instantiate(_sceneUIRootPrefab); //Загружаем UI из префаба
            uiRoot.AttachSceneUI(uiSceneRootBinder.gameObject);

            var uiSceneRootViewModel = viewsDiContainer.Resolve<UIGameplayRootViewModel>();
            uiSceneRootBinder.Bind(uiSceneRootViewModel);
            
            //можно открывать окошки
            var uiManager = viewsDiContainer.Resolve<GameplayUIManager>();
            uiManager.OpenScreenGameplay();

        }
    }
}