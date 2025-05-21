using System;
using Game.GamePlay.Root.View;
using Game.MainMenu.Root.View;
using Scripts.Game.GameRoot;
using UnityEngine;

namespace Game.MainMenu.Root
{
    public class MainMenuEntryPoint : MonoBehaviour
    {
        public event Action GoToGameplaySceneRequested;
        
        [SerializeField] private UIMainMenuRootBinder _sceneUIRootPrefab;

        public void Run(UIRootView uiRoot)
        {
            var uiScene = Instantiate(_sceneUIRootPrefab);
        //    Debug.Log(_sceneUIRootPrefab.gameObject.name);
            uiRoot.AttachSceneUI(uiScene.gameObject);
            
            uiScene.GoToGameplayButtonClicked += () =>
            {
                GoToGameplaySceneRequested?.Invoke();
            };

        }
    }
}