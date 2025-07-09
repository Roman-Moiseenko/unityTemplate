using MVVM.UI;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PanelActions
{
    public class PanelActionsBinder : PanelBinder<PanelActionsViewModel>
    {
        [SerializeField] private Button _btnGameSpeed;
        [SerializeField] private Button _btnProgressAdd;
 
        private void Start()
        {
        ViewModel.CurrentSpeed.Subscribe(x =>
        {
            _btnGameSpeed.GetComponentInChildren<TMP_Text>().text = $"{x}x";
        });
            //_btnGameSpeed.GetComponentInChildren<TMP_Text>().text = $"{ViewModel.CurrentSpeed}x";
        }
        
        private void OnEnable()
        {
            _btnGameSpeed.onClick.AddListener(OnChangeGameSpeed);
            _btnProgressAdd.onClick.AddListener(OnProgressAdd);
        }

        private void OnDisable()
        {
            _btnGameSpeed.onClick.RemoveListener(OnChangeGameSpeed);
            _btnProgressAdd.onClick.RemoveListener(OnProgressAdd);
        }

        private void OnChangeGameSpeed()
        {
            ViewModel.RequestGameSpeed();
        }
        private void OnProgressAdd()
        {
            ViewModel.RequestToProgressAdd();
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