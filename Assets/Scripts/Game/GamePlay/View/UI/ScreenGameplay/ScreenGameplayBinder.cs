using System;
using MVVM.UI;
using Newtonsoft.Json;
using R3;
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
        [SerializeField] private Button _btnPopupPause;
        [SerializeField] private TMP_Text _textProgress;
        [SerializeField] private TMP_Text _textMoney;
        [SerializeField] private TMP_Text _textCrystal;
        
        private void Start()
        {
            ViewModel.ProgressData.Subscribe(newValue =>
            {
                _textProgress.text = $"Progress: {newValue}";
            });
            ViewModel.SoftCurrency.Subscribe(newValue => _textMoney.text = $"Money: {newValue}");
            ViewModel.HardCurrency.Subscribe(newValue => _textCrystal.text = $"Money: {newValue}");
        }

        private void OnEnable()
        {
            _btnPopupPause.onClick.AddListener(OnPopupPauseButtonClicked);
        }

        private void OnDisable()
        {
            _btnPopupPause.onClick.RemoveListener(OnPopupPauseButtonClicked);
        }
        
        private void OnPopupBButtonClicked()
        {
            ViewModel.RequestOpenPopupB();
        }

        private void OnGoToMenuButtonClicked()
        {
            ViewModel.RequestGoToMainMenu();
        }

        private void OnPopupPauseButtonClicked()
        {
            ViewModel.RequestOpenPopupPause();
        }


        
    }
}