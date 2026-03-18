using System;
using System.Collections.Generic;
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
        [SerializeField] private Button btnGameSpeed;
        [SerializeField] private Button btnProgressAdd;
        [SerializeField] private Button btnBuySpeed4x;
        [SerializeField] private List<Transform> speedList;
        private IDisposable _disposable;

        protected override void OnBind(PanelActionsViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
           // _btnGameSpeed.GetComponentInChildren<TMP_Text>().text = $"{ViewModel.GetCurrentSpeed()}x";
            SetSpeed(ViewModel.GetCurrentSpeed());
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
            btnGameSpeed.onClick.AddListener(OnChangeGameSpeed);
            btnProgressAdd.onClick.AddListener(OnProgressAdd);
            btnBuySpeed4x.onClick.AddListener(OnBuySpeed4x);
        }

        private void OnDisable()
        {
            btnGameSpeed.onClick.RemoveListener(OnChangeGameSpeed);
            btnProgressAdd.onClick.RemoveListener(OnProgressAdd);
            btnBuySpeed4x.onClick.RemoveListener(OnBuySpeed4x);
        }

        private void OnChangeGameSpeed()
        {
            
            ViewModel.RequestGameSpeed();
            SetSpeed(ViewModel.GetCurrentSpeed());
            //_btnGameSpeed.GetComponentInChildren<TMP_Text>().text = $"x{ViewModel.GetCurrentSpeed()}";
        }

        private void OnBuySpeed4x()
        {
            ViewModel.RequestBuySpeed4x();
        }
        private void OnProgressAdd()
        {
            ViewModel.RequestToProgressAdd();
        }

        public override void Show()
        {
            panel.pivot = new Vector2(1f, 0.5f);
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
            panel.pivot = new Vector2(0f, 0.5f);
        }

        private void OnDestroy()
        {
            _disposable.Dispose();
        }

        private void SetSpeed(float speed)
        {
            var nameObj = "x" + speed;
            foreach (var speedTransform in speedList)
            {
                speedTransform.gameObject.SetActive(speedTransform.name == nameObj);
            }
            
        }
    }
}