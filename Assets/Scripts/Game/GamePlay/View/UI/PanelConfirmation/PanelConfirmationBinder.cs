using System;
using MVVM.UI;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PanelConfirmation
{
    public class PanelConfirmationBinder : PanelBinder<PanelConfirmationViewModel>
    {
        [SerializeField] private Button _btnConfirmation;
        [SerializeField] private Button _btnCancel;
        [SerializeField] private Button _btnRotate;
        private IDisposable _disposable;

        protected override void OnBind(PanelConfirmationViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            ViewModel.IsEnable.Subscribe(newValue =>
            {
                _btnConfirmation.interactable = newValue;
            }).AddTo(ref d);
            ViewModel.IsRotate.Subscribe(isRotation =>
            {
                _btnRotate.gameObject.SetActive(isRotation);
                var confirmY = _btnConfirmation.transform.localPosition;
                var cancelY = _btnCancel.transform.localPosition;
                //var rotateY = _btnRotate.transform.localPosition;
                if (isRotation)
                {
                    confirmY.y = 140f;
                    cancelY.y = -140f;
                }
                else
                {
                    confirmY.y = 70f;
                    cancelY.y = -70f;
                }

                _btnConfirmation.transform.localPosition = confirmY;
                _btnCancel.transform.localPosition = cancelY;
            }).AddTo(ref d);
            _disposable = d.Build();
        }
        
        private void OnEnable()
        {
            _btnConfirmation.onClick.AddListener(OnConfirmation);
            _btnCancel.onClick.AddListener(OnCancel);
            _btnRotate.onClick.AddListener(OnRotate);
        }

        private void OnDisable()
        {
            _btnConfirmation.onClick.RemoveListener(OnConfirmation);
            _btnCancel.onClick.RemoveListener(OnCancel);
            _btnRotate.onClick.RemoveListener(OnRotate);
        }

        private void OnConfirmation()
        {
            ViewModel.RequestConfirmation();
        }

        private void OnCancel()
        {
            ViewModel.RequestCancel();
        }

        private void OnRotate()
        {
            ViewModel.RequestRotate();
        }

        public override void Show()
        {
            panel.pivot = new Vector2(1f, 0.5f);
        }

        public override void Hide()
        {
            panel.pivot = new Vector2(0f, 0.5f);
        }
        
        private void OnDestroy()
        {
            _disposable.Dispose();
        }
    }
}