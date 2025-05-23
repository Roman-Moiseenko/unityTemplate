using System;
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

        public Observable<GameplayExitParams> Run(UIRootView uiRoot, GameplayEnterParams enterParams)
        {
            var uiScene = Instantiate(_sceneUIRootPrefab);
            uiRoot.AttachSceneUI(uiScene.gameObject);
            
            var exitSceneSignalSubj = new Subject<Unit>();
            uiScene.Bind(exitSceneSignalSubj);
            
            Debug.Log($"MAIN MENU ENTER POINT: Results {enterParams?.SaveFileName} and Level {enterParams?.LevelNumber}");

                //Создаем выходные параметры для входа в Меню
            var mainMenuEnterParams = new MainMenuEnterParams("Fatality");
            var exitParams = new GameplayExitParams(mainMenuEnterParams);
            
            var exitToMainMenuSignal = exitSceneSignalSubj.Select(_ => exitParams);
            return exitToMainMenuSignal;
        }
    }
}