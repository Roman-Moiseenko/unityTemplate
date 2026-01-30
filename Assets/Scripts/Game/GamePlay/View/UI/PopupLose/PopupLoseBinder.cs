using MVVM.UI;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PopupLose
{
    public class PopupLoseBinder  : PopupBinder<PopupLoseViewModel>
    {
        [SerializeField] private Button _btnPlayAd;
        [SerializeField] private Button _btnSpendCristal;

        protected override void OnBind(PopupLoseViewModel viewModel)
        {
            base.OnBind(viewModel);

            viewModel.ShowButtonAd.Subscribe(v =>
            {
                _btnPlayAd.gameObject.SetActive(v);
            });
        }

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
            ViewModel.RequestSpendCristal();
        }

        private void OnGoToPlayAdClicked()
        {
            ViewModel.RequestPlayAd();
        }

        protected override void OnCloseButtonClick()
        {
            //
            ViewModel.RequestClose();
        }
    }
}