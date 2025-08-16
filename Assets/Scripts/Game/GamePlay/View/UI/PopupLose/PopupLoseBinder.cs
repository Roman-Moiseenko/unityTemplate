using Game.GamePlay.View.UI.PopupPause;
using MVVM.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PopupLose
{
    public class PopupLoseBinder  : PopupBinder<PopupLoseViewModel>
    {
        [SerializeField] private Button _btnPlayAd;
        [SerializeField] private Button _btnSpendCristal;
        
        private void OnEnable()
        {
            _btnPlayAd.onClick.AddListener(OnGoToPlayAdClicked);
            _btnSpendCristal.onClick.AddListener(OnSpendCristalClicked);
        }

        private void OnDisable()
        {
            _btnPlayAd.onClick.RemoveListener(OnGoToPlayAdClicked);
            _btnSpendCristal.onClick.RemoveListener(OnSpendCristalClicked);
        }
        private void OnSpendCristalClicked()
        {
            ViewModel.RequestPlayAd();
        }

        private void OnGoToPlayAdClicked()
        {
            ViewModel.RequestSpendCristal();
        }

        protected override void OnCloseButtonClick()
        {
            //
            ViewModel.RequestClose();
        }
    }
}