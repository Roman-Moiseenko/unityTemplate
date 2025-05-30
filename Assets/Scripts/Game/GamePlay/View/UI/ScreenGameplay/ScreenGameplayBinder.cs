using System;
using MVVM.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.ScreenGameplay
{
    public class ScreenGameplayBinder : WindowBinder<ScreenGameplayViewModel>
    {
        [SerializeField] private Button _btnPopupA;
        [SerializeField] private Button _btnPopupB;
        [SerializeField] private Button _btnGoToMenu;

        private void OnEnable()
        {
            _btnPopupA.onClick.AddListener(OnPopupAButtonClicked);
            _btnPopupB.onClick.AddListener(OnPopupBButtonClicked);
            _btnGoToMenu.onClick.AddListener(OnGoToMenuButtonClicked);
        }

        private void OnDisable()
        {
            _btnPopupA.onClick.RemoveListener(OnPopupAButtonClicked);
            _btnPopupB.onClick.RemoveListener(OnPopupBButtonClicked);
            _btnGoToMenu.onClick.RemoveListener(OnGoToMenuButtonClicked);
        }

        private void OnPopupBButtonClicked()
        {
            ViewModel.RequestOpenPopupA();
        }

        private void OnGoToMenuButtonClicked()
        {
            ViewModel.RequestOpenPopupB();
        }

        private void OnPopupAButtonClicked()
        {
            ViewModel.RequestGoToMainMenu();
        }
    }
}