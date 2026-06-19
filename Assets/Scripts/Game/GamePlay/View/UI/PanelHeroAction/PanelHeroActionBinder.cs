using System;
using DG.Tweening;
using MVVM.UI;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PanelHeroAction
{
    public class PanelHeroActionBinder : PanelBinder<PanelHeroActionViewModel>
    {
        [SerializeField] private Button btnPlacement;
        private Vector2 _hidePivot = new Vector2(0f, 0.5f);
        private Vector2 _showPivot = new Vector2(1f, 0.5f);
        
        protected override void OnBind(PanelHeroActionViewModel viewModel)
        {
            btnPlacement.gameObject.SetActive(true);
        }
        
        private void OnEnable()
        {
            btnPlacement.onClick.AddListener(OnPlacement);
        }

        private void OnDisable()
        {
            btnPlacement.onClick.RemoveListener(OnPlacement);
        }

        private void OnPlacement()
        {
            ViewModel.RequestPlacement();
        }

        public override void Show()
        {
            panel.DOPivot(_showPivot, 0.3f).From(_hidePivot).SetUpdate(true);
        }

        public override void Hide()
        {
            panel.DOPivot(_hidePivot, 0.3f).From(_showPivot).SetUpdate(true);
        }
    }
}