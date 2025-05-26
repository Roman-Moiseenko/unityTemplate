using System;
using DI;
using Game.GamePlay.Commands;
using Game.GamePlay.Root.View;
using Game.GamePlay.Services;
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
        
        [SerializeField] private UIGameplayRootBinder _sceneUIRootPrefab;

        [SerializeField] private WorldGameplayRootBinder _worldRootBinder;
        public Observable<GameplayExitParams> Run(DIContainer gameplayContainer, GameplayEnterParams enterParams)
        {
            GameplayRegistrations.Register(gameplayContainer, enterParams); //Регистрируем все сервисы сцены
            var gameplayViewModelsContainer = new DIContainer(gameplayContainer); //Создаем контейнер для view-моделей
            GameplayViewModelsRegistrations.Register(gameplayViewModelsContainer);

            ///
            var gameStateProvider = gameplayContainer.Resolve<IGameStateProvider>();

            gameStateProvider.GameState.Buildings.ObserveAdd().Subscribe(e =>
            {
                var building = e.Value;
                Debug.Log("Здание размещено. TypeId " + building.TypeId + 
                          " Id = " + building.Id + 
                          "Position = " + building.Position.Value);
            });

            var buildingService = gameplayContainer.Resolve<BuildingsService>();

            buildingService.PlaceBuilding("Tower", GetRandomPosition());
            buildingService.PlaceBuilding("Tower", GetRandomPosition());
            buildingService.PlaceBuilding("Tower", GetRandomPosition());
            
            //для теста
            _worldRootBinder.Bind(gameplayViewModelsContainer.Resolve<WorldGameplayRootViewModel>());
            
            gameplayViewModelsContainer.Resolve<UIGameplayRootViewModel>();
            
            
            var uiScene = Instantiate(_sceneUIRootPrefab); //Загружаем UI из префаба
            var uiRoot = gameplayContainer.Resolve<UIRootView>(); //Находим рутовый контейнер с UI и присоединем загруженный UI
            uiRoot.AttachSceneUI(uiScene.gameObject);
            
            var exitSceneSignalSubj = new Subject<Unit>();
            uiScene.Bind(exitSceneSignalSubj);
            
            Debug.Log($"MAIN MENU ENTER POINT: Results {enterParams?.SaveFileName} and Level {enterParams?.LevelNumber}");

                //Создаем выходные параметры для входа в Меню
            var mainMenuEnterParams = new MainMenuEnterParams("Fatality");
            var exitParams = new GameplayExitParams(mainMenuEnterParams);
            //Формируем сигнал для подписки
            var exitToMainMenuSignal = exitSceneSignalSubj.Select(_ => exitParams);
            return exitToMainMenuSignal;
        }

        private Vector3Int GetRandomPosition()
        {
            var rX = Random.Range(-10, 10);
            var rY = Random.Range(-10, 10);
            return new Vector3Int(rX, rY, 0);
        }
    }
}