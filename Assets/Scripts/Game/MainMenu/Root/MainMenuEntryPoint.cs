using System;
using Game.GamePlay.Root;
using Game.GamePlay.Root.View;
using Game.MainMenu.Root.View;
using R3;
using Scripts.Game.GameRoot;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Game.MainMenu.Root
{
    public class MainMenuEntryPoint : MonoBehaviour
    {
        
        [SerializeField] private UIMainMenuRootBinder _sceneUIRootPrefab;

        public Observable<MainMenuExitParams> Run(UIRootView uiRoot, MainMenuEnterParams enterParams)
        {
            var uiScene = Instantiate(_sceneUIRootPrefab);
        //    Debug.Log(_sceneUIRootPrefab.gameObject.name);
            uiRoot.AttachSceneUI(uiScene.gameObject);
            
            var exitSceneSignalSubj = new Subject<Unit>();
            uiScene.Bind(exitSceneSignalSubj);
            
            Debug.Log($"MAIN MENU ENTER POINT: Results {enterParams?.Result}");
            var saveFileName = "file.save";
            var levelNumber = Random.Range(0, 100);
            var gameplayEnterParams = new GameplayEnterParams(saveFileName, levelNumber);
            var mainMenuExitParams = new MainMenuExitParams(gameplayEnterParams);
            var exitToGameplaySceneSignale = exitSceneSignalSubj.Select(_ => mainMenuExitParams);
            return exitToGameplaySceneSignale;

        }
    }
}