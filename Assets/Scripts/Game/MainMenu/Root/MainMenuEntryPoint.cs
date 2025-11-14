using System.Linq;
using DI;
using Game.Common;
using Game.GamePlay.Classes;
using Game.GamePlay.Root;
using Game.GamePlay.Services;
using Game.MainMenu.Root.View;
using Game.MainMenu.Services;
using Game.MainMenu.View;
using Game.MainMenu.View.MainScreen;
using Game.MainMenu.View.ScreenPlay;
using Game.State;
using Game.State.GameResources;
using Game.State.Inventory;
using Game.State.Inventory.TowerCards;
using MVVM.CMD;
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
            
            MainMenuRegistrations.Register(mainMenuContainer, enterParams); //Регистрируем все сервисы сцены меню
            var mainMenuViewModelsContainer = new DIContainer(mainMenuContainer); //Создаем контейнер для view-моделей
            MainMenuViewModelsRegistrations.Register(mainMenuViewModelsContainer);


            var gameState = mainMenuContainer.Resolve<IGameStateProvider>().GameState;
            //  mainMenuViewModelsContainer.Resolve<UIMainMenuRootViewModel>();
             
            
            InitUI(mainMenuViewModelsContainer);
            
            if (enterParams != null)
            {
                //Сервис обработки наград после геймплея
                var inventory = mainMenuContainer.Resolve<InventoryService>();
                if (enterParams.TypeGameplay == TypeGameplay.Infinity) inventory.InfinityRewardGamePlay(enterParams);
                //enterParams сохраняет дополнительные данные, для передачи в popup

                if (enterParams.TypeGameplay == TypeGameplay.Levels) inventory.LevelsRewardGamePlay(enterParams);
                    
                
                //Сохраняем параметры/настройки для следующих геймплеев
                var gameProvider = mainMenuContainer.Resolve<IGameStateProvider>();
                gameProvider.GameState.GameSpeed.Value = enterParams.GameSpeed;
                
                gameProvider.SaveGameState();

            }
            
            
            InitPopupPlay(mainMenuViewModelsContainer, enterParams);
            
//            Debug.Log($"MAIN MENU ENTER POINT: Results {enterParams?.Result}");
            
            //var gameStateProvider = mainMenuViewModelsContainer.Resolve<IGameStateProvider>(); //Получаем репозиторий
           // var gameState = gameStateProvider.GameState;
            
            //Debug.Log("Инвентарь " + JsonConvert.SerializeObject(gameState._gameState.InventoryItems, Formatting.Indented));

            
            //Загружаем входные параметры
            //   var gameplayEnterParams = new GameplayEnterParams(0); //Имитация выбора уровня 0
            //  var mainMenuExitParams = new MainMenuExitParams(gameplayEnterParams);
            return mainMenuContainer.Resolve<Subject<MainMenuExitParams>>();
            
            //var exitToGameplaySceneSignal = exitSceneRequest.Select(_ => mainMenuExitParams);
            //exitToGameplaySceneSignal.
            //  Debug.Log(JsonConvert.SerializeObject(mainMenuExitParams, Formatting.Indented));
            //return exitSceneRequest; //exitToGameplaySceneSignal;
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

            var mainScreenViewModel = new MainScreenViewModel(uiManager, container);
            var binder = uiSceneRootBinder.GetComponent<MainScreenBinder>();
            binder.Bind(mainScreenViewModel);

            uiManager.OpenScreenPlay();
        }

        private void InitPopupPlay(DIContainer container, MainMenuEnterParams enterParams)
        {
            var uiManager = container.Resolve<PlayUIManager>();
            
            
            if (enterParams != null)
            {
                uiManager.OpenPopupFinishGameplay(enterParams);    
            }
            
            
           // var uiManager = container.Resolve<MainMenuUIManager>();
            //TODO Сделать проверку, что есть сохраненные данные о сессии
            
            
            //Запрашиваем окно и переходим в игру или очищаем
            return;
            /*
            var gameplayState = container.Resolve<IGameStateProvider>().GameplayState;
            if (gameplayState.Entities.Any()) //если была сессия игры
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
            */
        }
    }
}