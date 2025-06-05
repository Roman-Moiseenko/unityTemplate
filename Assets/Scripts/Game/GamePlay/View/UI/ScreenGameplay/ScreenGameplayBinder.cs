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
        [SerializeField] private Button _btnGameSpeed;
        [SerializeField] private RectTransform _panelAction;
        [SerializeField] private RectTransform _panelBuild;
        
        /**
         * Режим строительства, временная кнопка, вызывается по событию.
         */
        [SerializeField] private Button _btnBuild;

        private void Start()
        {
            _btnGameSpeed.GetComponentInChildren<TMP_Text>().text = $"{ViewModel.CurrentSpeed}x";
        }

        private void OnEnable()
        {
            _btnPopupA.onClick.AddListener(OnPopupAButtonClicked);
            _btnPopupB.onClick.AddListener(OnPopupBButtonClicked);
            _btnGoToMenu.onClick.AddListener(OnGoToMenuButtonClicked);
            _btnGameSpeed.onClick.AddListener(OnChangeGameSpeed);
            _btnBuild.onClick.AddListener(OnBuild);
        }

        private void OnDisable()
        {
            _btnPopupA.onClick.RemoveListener(OnPopupAButtonClicked);
            _btnPopupB.onClick.RemoveListener(OnPopupBButtonClicked);
            _btnGoToMenu.onClick.RemoveListener(OnGoToMenuButtonClicked);
            _btnGameSpeed.onClick.RemoveListener(OnChangeGameSpeed);
            _btnBuild.onClick.RemoveListener(OnBuild);
        }

        private void OnBuild()
        {
            ViewModel.RequestToBuild();
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

        private void OnChangeGameSpeed()
        {
            _btnGameSpeed.GetComponentInChildren<TMP_Text>().text = $"{ViewModel.RequestGameSpeed()}x";
        }
    }
}