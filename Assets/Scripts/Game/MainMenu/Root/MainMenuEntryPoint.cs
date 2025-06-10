using DI;
using Game.Common;
using Game.GamePlay.Root;
using Game.GamePlay.Services;
using Game.MainMenu.Root.View;
using Game.MainMenu.View;
using Game.MainMenu.View.MainScreen;
using Game.State;
using Game.State.GameResources;
using Game.State.Inventory;
using Newtonsoft.Json;
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
            Debug.Log(" MainMenuEnterParams " + JsonConvert.SerializeObject(enterParams, Formatting.Indented));
            MainMenuRegistrations.Register(mainMenuContainer, enterParams); //Регистрируем все сервисы сцены меню
            var mainMenuViewModelsContainer = new DIContainer(mainMenuContainer); //Создаем контейнер для view-моделей
            MainMenuViewModelsRegistrations.Register(mainMenuViewModelsContainer);
            //TODO Сохранить входные данные в GameState если вышли из Игры 

            //  mainMenuViewModelsContainer.Resolve<UIMainMenuRootViewModel>();

            if (enterParams != null)
            {
                //TODO Добавляем награды
                var resourcesService = mainMenuContainer.Resolve<ResourcesService>();
                resourcesService.AddResource(ResourceType.SoftCurrency, enterParams.SoftCurrency);
                
  /*              var inventoryService = mainMenuContainer.Resolve<InventoryService>();
                enterParams.Inventory.ForEach(item =>
                {
                    InventoryService.AddInventory(InventoryFactory.CreateInventory(item));
                });
*/
                
            }
            
            InitUI(mainMenuViewModelsContainer);

            InitPopup(mainMenuViewModelsContainer);


            // var exitSceneSignalSubj = new Subject<Unit>();
            //   uiScene.Bind(exitSceneSignalSubj);

            Debug.Log($"MAIN MENU ENTER POINT: Results {enterParams?.Result}");

            //Загружаем входные параметры
            //   var gameplayEnterParams = new GameplayEnterParams(0); //Имитация выбора уровня 0
            //  var mainMenuExitParams = new MainMenuExitParams(gameplayEnterParams);
            var exitSceneRequest =
                mainMenuContainer.Resolve<Subject<MainMenuExitParams>>(AppConstants.EXIT_SCENE_REQUEST_TAG);
            //var exitToGameplaySceneSignal = exitSceneRequest.Select(_ => mainMenuExitParams);
            //exitToGameplaySceneSignal.
            //  Debug.Log(JsonConvert.SerializeObject(mainMenuExitParams, Formatting.Indented));
            return exitSceneRequest; //exitToGameplaySceneSignal;
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

            uiManager.OpenScreenPlay();
        }

        private void InitPopup(DIContainer container)
        {
            var uiManager = container.Resolve<MainMenuUIManager>();
            var gameState = container.Resolve<IGameStateProvider>().GameState;
            if (gameState.HasSessionGame.Value) //если была сессия игры
            {
                //Окно вернуться к ней
                uiManager.OpenPopupResumeGame();
            }
            else
            {
                //Иначе рекламные окна
                //TODO
                Debug.Log("Открыть рекламное окно");
                //uiManager.OpenPopupAD_01();
                //uiManager.OpenPopupAD_02();
                //uiManager.OpenPopupAD_03();
                //uiManager.OpenPopupAD_04();
            }
        }
    }
}