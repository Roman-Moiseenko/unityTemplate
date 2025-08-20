using System;
using System.Collections.Generic;
using Game.Common;
using MVVM.UI;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PanelBuild
{
    public class PanelBuildBinder : PanelBinder<PanelBuildViewModel>
    {
        [SerializeField] private Button _btnBuild1;
        [SerializeField] private Button _btnBuild2;
        [SerializeField] private Button _btnBuild3;
        [SerializeField] private Button _btnUpdate;

        private Dictionary<int, Button> Buttons = new();
        private Transform _freeCaption;
        private Transform _paidCaption;

        private IDisposable _disposable;
        private void Awake()
        {
            _freeCaption = _btnUpdate.transform.Find("Free");
            _paidCaption = _btnUpdate.transform.Find("Paid");
        }

        protected override void OnBind(PanelBuildViewModel viewModel)
        {
            var d = Disposable.CreateBuilder();
            _freeCaption.gameObject.SetActive(true);
            _paidCaption.gameObject.SetActive(false);
            
            Buttons.Add(1, _btnBuild1);
            Buttons.Add(2, _btnBuild2);
            Buttons.Add(3, _btnBuild3);

            viewModel.UpdateCards.Subscribe(value =>
            {
                if (value == 0)
                {
                    _freeCaption.gameObject.SetActive(true);
                    _paidCaption.gameObject.SetActive(false);
                }
                else
                {
                    _freeCaption.gameObject.SetActive(false);
                    _paidCaption.gameObject.SetActive(true);
                    _paidCaption.transform.Find("costText").gameObject.GetComponent<TMP_Text>().text = 
                        (value * AppConstants.COST_UPDATE_BUILD).ToString();
                }
            }).AddTo(ref d);
            
            foreach (var buttonCard in viewModel.ButtonCards)
            {
                if (Buttons.TryGetValue(buttonCard.Key, out var button))
                    UpdateTextButton(button, buttonCard.Value);
            }

            viewModel.ButtonCards.ObserveAdd().Subscribe(e =>
            {
                var indexButton = e.Value.Key;
                var buttonData = e.Value.Value;
                if (Buttons.TryGetValue(indexButton, out var button)) UpdateTextButton(button, buttonData);
            }).AddTo(ref d);
            _disposable = d.Build();
        }

        private void UpdateTextButton(Button button, ButtonData buttonData)
        {
            button.transform.Find("Caption").GetComponentInChildren<TMP_Text>().text = buttonData.Caption;
            button.transform.Find("Level").GetComponentInChildren<TMP_Text>().text = buttonData.Level;
            button.transform.Find("Description").GetComponentInChildren<TMP_Text>().text = buttonData.Description;

            if (buttonData.PrehabImage != "")
            {
                button.transform.Find("Image").GetComponentInChildren<Image>().sprite =
                    Resources.Load<Sprite>($"Images/{buttonData.PrehabImage}");
            }
        }

        public override void Show()
        {
            if (isShow) return;
            //Получаем у ViewModel данные для отображения на карточках, грузим картинки
            base.Show();
            panel.pivot = new Vector2(0.5f, 0);
        }

        public override void Hide()
        {
            if (!isShow) return;
            base.Hide();
            panel.pivot = new Vector2(0.5f, 1);
        }


        private void OnEnable()
        {
            _btnBuild1.onClick.AddListener(OnClickButtonBuild1);
            _btnBuild2.onClick.AddListener(OnClickButtonBuild2);
            _btnBuild3.onClick.AddListener(OnClickButtonBuild3);
            _btnUpdate.onClick.AddListener(OnClickUpdate);
        }


        private void OnDisable()
        {
            _btnBuild1.onClick.RemoveListener(OnClickButtonBuild1);
            _btnBuild2.onClick.RemoveListener(OnClickButtonBuild2);
            _btnBuild3.onClick.RemoveListener(OnClickButtonBuild3);
            _btnUpdate.onClick.RemoveListener(OnClickUpdate);
        }

        private void OnClickUpdate()
        {
            ViewModel.OnUpdateCard();
        }

        private void OnClickButtonBuild1()
        {
            ViewModel.OnBuild1();
        }

        private void OnClickButtonBuild2()
        {
            ViewModel.OnBuild2();
        }

        private void OnClickButtonBuild3()
        {
            ViewModel.OnBuild3();
        }
        
        public void OnDestroy()
        {
            _disposable.Dispose();
        }
    }
}