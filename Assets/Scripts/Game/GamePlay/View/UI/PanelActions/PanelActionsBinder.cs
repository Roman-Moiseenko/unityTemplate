using MVVM.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PanelActions
{
    public class PanelActionsBinder : PanelBinder<PanelActionsViewModel>
    {
        [SerializeField] private Button _btnGameSpeed;
        [SerializeField] private Button _btnBuild;
        
        
        
        private void Start()
        {
            _btnGameSpeed.GetComponentInChildren<TMP_Text>().text = $"{ViewModel.CurrentSpeed}x";
        }
        
        private void OnEnable()
        {
            _btnGameSpeed.onClick.AddListener(OnChangeGameSpeed);
            _btnBuild.onClick.AddListener(OnBuild);
        }

        private void OnDisable()
        {
            _btnGameSpeed.onClick.RemoveListener(OnChangeGameSpeed);
            _btnBuild.onClick.RemoveListener(OnBuild);
        }
        
        private void OnChangeGameSpeed()
        {
            _btnGameSpeed.GetComponentInChildren<TMP_Text>().text = $"{ViewModel.RequestGameSpeed()}x";
        }
        private void OnBuild()
        {
            ViewModel.RequestToBuild();
        }
        
        public override void Show()
        {
            if (isShow) return;
            panel.pivot = new Vector2(1f, 0.5f);
            base.Show();
        }
        
        public override void Hide()
        {
            if (!isShow) return;
            base.Hide();
            panel.pivot = new Vector2(0f, 0.5f);
        }
    }
}