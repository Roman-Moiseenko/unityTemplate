using System;
using DI;
using Game.GamePlay.Root.View;
using Game.MainMenu.Root;
using R3;
using Scripts.Game.GameRoot;
using UnityEngine;

namespace Game.GamePlay.Root
{
    public class GameplayEntryPoint : MonoBehaviour
    {
        
        [SerializeField] private UIGameplayRootBinder _sceneUIRootPrefab;

        public Observable<GameplayExitParams> Run(DIContainer gameplayContainer, GameplayEnterParams enterParams)
        {
            GameplayRegistrations.Register(gameplayContainer, enterParams); //Регистрируем все сервисы сцены
            var gameplayViewModelsContainer = new DIContainer(gameplayContainer); //Создаем контейнер для view-моделей
            GameplayViewModelsRegistrations.Register(gameplayViewModelsContainer);


            gameplayViewModelsContainer.Resolve<UIGameplayRootViewModel>();
            gameplayViewModelsContainer.Resolve<WorldGameplayRootViewModel>();
            
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
    }
}