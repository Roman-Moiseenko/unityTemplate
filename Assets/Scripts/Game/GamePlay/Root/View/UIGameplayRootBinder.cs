using System;
using UnityEngine;

namespace Game.GamePlay.Root.View
{
    public class UIGameplayRootBinder : MonoBehaviour
    {
        public event Action GoToMainMenuButtonClicked;

        public void HandleGoToMainMenuButtonClicked()
        {
            GoToMainMenuButtonClicked?.Invoke();
        }
    }
}
