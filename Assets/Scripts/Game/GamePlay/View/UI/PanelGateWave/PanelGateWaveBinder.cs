using MVVM.UI;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PanelGateWave
{
    public class PanelGateWaveBinder : PanelBinder<PanelGateWaveViewModel>
    {
        [SerializeField] private Button _btnInfo;
        [SerializeField] private Transform _infoBlock;
        private Image _btnImage;
        private Animator _animator;
        
        private void Start()
        {
            _btnImage = _btnInfo.GetComponent<Image>();
            _btnImage.fillAmount = 1;
            _animator = gameObject.GetComponent<Animator>();
            
            ViewModel.ShowInfoWave.Subscribe(showInfo =>
            {
                if (showInfo)
                {
                    _infoBlock.gameObject.SetActive(true);  
                    _animator.Play("info_wave_start");
                }
                else
                {
                    _animator.Play("info_wave_finish");
                }
                
                _btnImage.fillAmount = 1;
                ViewModel.IsSelected.Value = false;
            });
            ViewModel.PositionInfoBtn.Subscribe(p =>
            {
                _infoBlock.transform.position = p;
            });
            ViewModel.FillAmountBtn.Subscribe(n =>
            {
                _btnImage.fillAmount = n;
            });
        }
        
        private void OnEnable()
        {
            _btnInfo.onClick.AddListener(OnStartForced);
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
            if (!ViewModel.IsSelected.CurrentValue)
            {
                ViewModel.IsSelected.Value = true;
            } else
            {
                ViewModel.StartForcedWave();
            }
        }
        
        public override void Show()
        {
            
        }
        
        public override void Hide()
        {
            
        }

        public void EndFinishAnimation()
        {
            _infoBlock.gameObject.SetActive(false);
            //Пауза 0.5с
        }
    }
}