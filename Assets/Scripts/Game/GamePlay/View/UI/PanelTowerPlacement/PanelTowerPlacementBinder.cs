using System;
using MVVM.UI;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PanelTowerPlacement
{
    public class PanelTowerPlacementBinder : PanelBinder<PanelTowerPlacementViewModel>
    {
        [SerializeField] private Button btnConfirmation;
        [SerializeField] private Button btnCancel;
        private IDisposable _disposable;

        protected override void OnBind(PanelTowerPlacementViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            ViewModel.IsEnable.Subscribe(newValue =>
            {
                btnConfirmation.interactable = newValue;
            }).AddTo(ref d);
            
            _disposable = d.Build();
        }
        
                
        private void OnEnable()
        {
            btnConfirmation.onClick.AddListener(OnConfirmation);
            btnCancel.onClick.AddListener(OnCancel);
        }

        private void OnDisable()
        {
            btnConfirmation.onClick.RemoveListener(OnConfirmation);
            btnCancel.onClick.RemoveListener(OnCancel);
        }

        private void OnConfirmation()
        {
            ViewModel.RequestConfirmation();
        }

        private void OnCancel()
        {
            ViewModel.RequestCancel();
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