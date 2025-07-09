using MVVM.UI;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PanelGateWave
{
    public class PanelGateWaveBinder : PanelBinder<PanelGateWaveViewModel>
    {
        [SerializeField] private Button _btnInfo;
        
        private void Start()
        {
            ViewModel.ShowGate.Subscribe(h =>
            {
                _btnInfo.gameObject.SetActive(!h);
            });
            ViewModel.PositionInfoBtn.Subscribe(p =>
            {
                _btnInfo.transform.position = p;
            });
            //TODO Подписка надвижение камеры и смещение кнопки
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