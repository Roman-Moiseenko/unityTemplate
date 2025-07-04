﻿using Game.GamePlay.View.UI.PanelActions;
using MVVM.UI;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PanelConfirmation
{
    public class PanelConfirmationBinder : PanelBinder<PanelConfirmationViewModel>
    {
        [SerializeField] private Button _btnConfirmation;
        [SerializeField] private Button _btnCancel;
        [SerializeField] private Button _btnRotate;


        protected override void OnBind(PanelConfirmationViewModel viewModel)
        {
            //TODO Проверяем, нужен ли поворот и показываем/прячем кнопку
            ViewModel.IsEnable.Subscribe(newValue =>
            {
                _btnConfirmation.interactable = newValue;
            });
            ViewModel.IsRotate.Subscribe(newValue =>
            {
                _btnRotate.gameObject.SetActive(newValue);
                var vector3 = _btnCancel.transform.localPosition;
                if (newValue)
                {
                    vector3.y = -70;
                }
                else
                {
                    vector3.y = 0;
                }
                _btnCancel.transform.localPosition = vector3;
            });
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