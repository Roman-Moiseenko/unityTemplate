using MVVM.UI;
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
  //      [SerializeField] private Button _btnSoftCurrencyAdd;
 //       [SerializeField] private Button _btnHardCurrencyAdd;
 
        private void Start()
        {
            _btnGameSpeed.GetComponentInChildren<TMP_Text>().text = $"{ViewModel.CurrentSpeed}x";
        }
        
        private void OnEnable()
        {
            _btnGameSpeed.onClick.AddListener(OnChangeGameSpeed);
            _btnProgressAdd.onClick.AddListener(OnProgressAdd);
    //        _btnSoftCurrencyAdd.onClick.AddListener(OnSoftCurrencyAdd);
   //         _btnHardCurrencyAdd.onClick.AddListener(OnHardCurrencyAdd);
        }

        private void OnDisable()
        {
            _btnGameSpeed.onClick.RemoveListener(OnChangeGameSpeed);
            _btnProgressAdd.onClick.RemoveListener(OnProgressAdd);
//            _btnSoftCurrencyAdd.onClick.RemoveListener(OnSoftCurrencyAdd);
 //           _btnHardCurrencyAdd.onClick.RemoveListener(OnHardCurrencyAdd);
        }

        private void OnHardCurrencyAdd()
        {
            ViewModel.RequestToHardCurrencyAdd();
        }

        private void OnSoftCurrencyAdd()
        {
            ViewModel.RequestToSoftCurrencyAdd();
        }

        private void OnChangeGameSpeed()
        {
            _btnGameSpeed.GetComponentInChildren<TMP_Text>().text = $"{ViewModel.RequestGameSpeed()}x";
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