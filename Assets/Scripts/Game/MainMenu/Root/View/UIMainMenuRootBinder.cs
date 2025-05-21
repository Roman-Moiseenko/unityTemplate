using System;
using UnityEngine;

namespace Game.MainMenu.Root.View
{
    public class UIMainMenuRootBinder : MonoBehaviour
    {
        public event Action GoToGameplayButtonClicked;

        public void HandleGoToGameplayButtonClicked()
        {
            GoToGameplayButtonClicked?.Invoke();
        }
    }
}