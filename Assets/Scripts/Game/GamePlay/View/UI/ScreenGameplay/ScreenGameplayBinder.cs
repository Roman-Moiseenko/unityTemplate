using System;
using MVVM.UI;
using Newtonsoft.Json;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Object = UnityEngine.Object;

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
            ViewModel.RequestOpenPopupB();
        }

        private void OnGoToMenuButtonClicked()
        {
            ViewModel.RequestGoToMainMenu();
        }

        private void OnPopupAButtonClicked()
        {
            ViewModel.RequestOpenPopupA();
        }


        
    }
}