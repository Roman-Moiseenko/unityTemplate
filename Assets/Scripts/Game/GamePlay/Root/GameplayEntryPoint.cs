using System;
using Game.GamePlay.Root.View;
using Scripts.Game.GameRoot;
using UnityEngine;

namespace Game.GamePlay.Root
{
    public class GameplayEntryPoint : MonoBehaviour
    {
        public event Action GoToMainMenuSceneRequested;
        
        [SerializeField] private UIGameplayRootBinder _sceneUIRootPrefab;

        public void Run(UIRootView uiRoot)
        {
            var uiScene = Instantiate(_sceneUIRootPrefab);
            uiRoot.AttachSceneUI(uiScene.gameObject);
            uiScene.GoToMainMenuButtonClicked += () =>
            {
                GoToMainMenuSceneRequested?.Invoke();
            };

        }
    }
}