using System;
using DG.Tweening;
using Game.GamePlay.View.UI.PanelConfirmation;
using MVVM.UI;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PanelTowerAction
{
    public class PanelTowerActionBinder : PanelBinder<PanelTowerActionViewModel>
    {
        [SerializeField] private Button btnPlacement;
        [SerializeField] private Button btnRemove;
        private IDisposable _disposable;
        private Vector2 _hidePivot = new Vector2(0f, 0.5f);
        private Vector2 _showPivot = new Vector2(1f, 0.5f);
        
        protected override void OnBind(PanelTowerActionViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();

            ViewModel.IsPlacement.Subscribe(newValue =>
            {
                btnPlacement.gameObject.SetActive(newValue);
            /*    var vector3 = btnRemove.transform.localPosition;
                if (newValue)
                {
                    vector3.y = -110;
                }
                else
                {
                    vector3.y = 0;
                }
                btnRemove.transform.localPosition = vector3;*/
            }).AddTo(ref d);
            _disposable = d.Build();
        }
        
        private void OnEnable()
        {
            btnPlacement.onClick.AddListener(OnPlacement);
            btnRemove.onClick.AddListener(OnRemove);
        }

        private void OnDisable()
        {
            btnPlacement.onClick.RemoveListener(OnPlacement);
            btnRemove.onClick.RemoveListener(OnRemove);
        }

        private void OnPlacement()
        {
            ViewModel.RequestPlacement();
        }

        private void OnRemove()
        {
            ViewModel.RequestRemove();
        }

        public override void Show()
        {
            panel.DOPivot(_showPivot, 0.3f).From(_hidePivot).SetUpdate(true);
            //panel.pivot
          //  panel.pivot = new Vector2(1f, 0.5f);
        }

        public override void Hide()
        {
            panel.DOPivot(_hidePivot, 0.3f).From(_showPivot).SetUpdate(true);
          //  panel.pivot = new Vector2(0f, 0.5f);
        }
        
        private void OnDestroy()
        {
            _disposable.Dispose();
        }
    }
}