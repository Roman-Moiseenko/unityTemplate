using System;
using MVVM.UI;
using R3;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PanelGateWave
{
    public class PanelGateWaveBinder : PanelBinder<PanelGateWaveViewModel>
    {
        [SerializeField] private Button _btnInfo;
        [SerializeField] private Transform _infoBlock;
        private Image _btnImage;
        
        private void Start()
        {
            _btnImage = _btnInfo.GetComponent<Image>();
            _btnImage.fillAmount = 1;
            
            ViewModel.ShowGate.Subscribe(h =>
            {
                _infoBlock.gameObject.SetActive(!h);
                _btnImage.fillAmount = 1;
            });
            ViewModel.PositionInfoBtn.Subscribe(p =>
            {
                _infoBlock.transform.position = p;
            });
            ViewModel.FillAmountBtn.Subscribe(n =>
            {
                _btnImage.fillAmount = n;
            });
            //TODO Подписка надвижение камеры и смещение кнопки
        }

        private void Update()
        {
            //_btnImage.fillAmount = 
        }

        private void OnEnable()
        {
            _btnInfo.onClick.AddListener(OnStartForced);
            //_btnInfo.
        }

        private void OnDisable()
        {
            _btnInfo.onClick.RemoveListener(OnStartForced);
        }
        
        private void OnShowInfo()
        {
            ViewModel.ShowPopupInfo();
        }
        private void OnStartForced()
        {
            ViewModel.StartForcedWave();
        }
        
        public override void Show()
        {
        }
        
        public override void Hide()
        {
        }
    }
}