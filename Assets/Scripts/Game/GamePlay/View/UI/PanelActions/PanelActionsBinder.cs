using System;
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
        private IDisposable _disposable;

        protected override void OnBind(PanelActionsViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            _btnGameSpeed.GetComponentInChildren<TMP_Text>().text = $"{ViewModel.GetCurrentSpeed()}x";
            /*
            viewModel.CurrentSpeed
                .Subscribe(x => { _btnGameSpeed.GetComponentInChildren<TMP_Text>().text = $"{x}x"; })
                .AddTo(ref d);
            */
            _disposable = d.Build();
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
            _btnGameSpeed.GetComponentInChildren<TMP_Text>().text = $"{ViewModel.GetCurrentSpeed()}x";
        }

        private void OnProgressAdd()
        {
            ViewModel.RequestToProgressAdd();
        }

        public override void Show()
        {
            if (IsShow()) return;
            panel.pivot = new Vector2(1f, 0.5f);
            base.Show();
        }

        public override void Hide()
        {
            if (!IsShow()) return;
            base.Hide();
            panel.pivot = new Vector2(0f, 0.5f);
        }

        private void OnDestroy()
        {
            _disposable.Dispose();
        }
    }
}