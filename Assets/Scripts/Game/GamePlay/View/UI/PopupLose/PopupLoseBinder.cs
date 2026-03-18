using System.Collections;
using MVVM.UI;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PopupLose
{
    public class PopupLoseBinder  : PopupBinder<PopupLoseViewModel>
    {
        [SerializeField] private Button _btnPlayAd;
        [SerializeField] private Button _btnSpendCristal;
        [SerializeField] private TMP_Text txtTimer;
        [SerializeField] private Image imageTimer;
        [SerializeField] private Transform payCrystal;
        [SerializeField] private Transform payHeart;
        [SerializeField] private TMP_Text txtHeart;
        private Coroutine _coroutine;
        private int _time;

        protected override void OnBind(PopupLoseViewModel viewModel)
        {
            base.OnBind(viewModel);

            viewModel.ShowButtonAd.Subscribe(v =>
            {
                _btnPlayAd.gameObject.SetActive(v);
            });
            _time = 100;
            txtTimer.text = "10";
            imageTimer.fillAmount = 1;
            _coroutine = StartCoroutine(StartTimer());
            if (viewModel.CountHearts > 0)
            {
                txtHeart.text = $"{viewModel.CountHearts}/1";
                payCrystal.gameObject.SetActive(false);
                payHeart.gameObject.SetActive(true);
            }
            else
            {
                payCrystal.gameObject.SetActive(true);
                payHeart.gameObject.SetActive(false);
            }
        }

        private IEnumerator StartTimer()
        {
            _time--;
            txtTimer.text = (_time / 10).ToString();
            imageTimer.fillAmount = _time / 100f;
            yield return new WaitForSecondsRealtime(0.1f);
            
            if (_time > 0)
            {
                _coroutine = StartCoroutine(StartTimer());
            }
            else
            {
                OnCloseButtonClick();
            }
        }

        private void OnEnable()
        {
            _btnPlayAd.onClick.AddListener(OnGoToPlayAdClicked);
            _btnSpendCristal.onClick.AddListener(OnSpendCristalClicked);
        }
        
        //private Enum

        private void OnDisable()
        {
            _btnPlayAd.onClick.RemoveListener(OnGoToPlayAdClicked);
            _btnSpendCristal.onClick.RemoveListener(OnSpendCristalClicked);
            
        }
        private void OnSpendCristalClicked()
        {
            StopCoroutine(_coroutine);
            ViewModel.RequestSpendCristal();
        }

        private void OnGoToPlayAdClicked()
        {
            StopCoroutine(_coroutine);
            ViewModel.RequestPlayAd();
        }

        protected override void OnCloseButtonClick()
        {
            //
            ViewModel.RequestClose();
        }
    }
}