
using DI;
using Game.Common;
using Game.GamePlay.Root;

using Game.MainMenu.Root.View;
using Game.MainMenu.View;
using Game.MainMenu.View.MainScreen;
using R3;
using Scripts.Game.GameRoot;
using UnityEngine;

namespace Game.MainMenu.Root
{
    public class MainMenuEntryPoint : MonoBehaviour
    {
        [SerializeField] private UIMainMenuRootBinder _sceneUIRootPrefab;

        public Observable<MainMenuExitParams> Run(DIContainer mainMenuContainer, MainMenuEnterParams enterParams)
        {
            MainMenuRegistrations.Register(mainMenuContainer, enterParams); //Регистрируем все сервисы сцены меню
            var mainMenuViewModelsContainer = new DIContainer(mainMenuContainer); //Создаем контейнер для view-моделей
            MainMenuViewModelsRegistrations.Register(mainMenuViewModelsContainer);
            
          //  mainMenuViewModelsContainer.Resolve<UIMainMenuRootViewModel>();

            InitUI(mainMenuViewModelsContainer);
            

           // var exitSceneSignalSubj = new Subject<Unit>();
                //   uiScene.Bind(exitSceneSignalSubj);

            Debug.Log($"MAIN MENU ENTER POINT: Results {enterParams?.Result}");

            var gameplayEnterParams = new GameplayEnterParams(0); //Имитация выбора уровня 0
            var mainMenuExitParams = new MainMenuExitParams(gameplayEnterParams);
            var exitSceneRequest = mainMenuContainer.Resolve<Subject<Unit>>(AppConstants.EXIT_SCENE_REQUEST_TAG);
            var exitToGameplaySceneSignal = exitSceneRequest.Select(_ => mainMenuExitParams);
            return exitToGameplaySceneSignal;
        }

        private void InitUI(DIContainer container)
        {
            var uiRoot = container.Resolve<UIRootView>();
            var uiSceneRootBinder = Instantiate(_sceneUIRootPrefab);

            //    Debug.Log(_sceneUIRootPrefab.gameObject.name);
            uiRoot.AttachSceneUI(uiSceneRootBinder.gameObject);
            
            var uiSceneRootViewModel = container.Resolve<UIMainMenuRootViewModel>();
            uiSceneRootBinder.Bind(uiSceneRootViewModel);
            
            var uiManager = container.Resolve<MainMenuUIManager>();
            
            var mainScreenViewModel = new MainScreenViewModel(uiManager);
            var Binder = uiSceneRootBinder.GetComponent<MainScreenBinder>();
            Binder.Bind(mainScreenViewModel);
            
            uiManager.OpenScreenShop();
        }
    }
}