using System;
using UnityEngine;
using R3;

namespace Game.MainMenu.Root.View
{
    public class UIMainMenuRootBinder : MonoBehaviour
    {
        private Subject<Unit> _exitSceneSignalSubj;
        
        public void HandleGoToGameplayButtonClicked()
        {
            _exitSceneSignalSubj?.OnNext(Unit.Default);
        }
        public void Bind(Subject<Unit> exitSceneSignalSubj)
        {
            _exitSceneSignalSubj = exitSceneSignalSubj;
        }
    }
    
}