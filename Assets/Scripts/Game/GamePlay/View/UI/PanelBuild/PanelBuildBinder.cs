using System;
using System.Collections.Generic;
using MVVM.UI;
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
        private Dictionary<int, Button> Buttons = new();
        
        private void Start()
        {
            Buttons.Add(1, _btnBuild1);
            Buttons.Add(2, _btnBuild2);
            Buttons.Add(3, _btnBuild3);
            foreach (var buttonCard in ViewModel.ButtonCards)
            {
                if (Buttons.TryGetValue(buttonCard.Key, out var button))
                    UpdateTextButton(button, buttonCard.Value);
            }
            
            ViewModel.ButtonCards.ObserveAdd().Subscribe(e =>
            {
                var indexButton = e.Value.Key;
                var buttonData = e.Value.Value;
                if (Buttons.TryGetValue(indexButton, out var button))
                    UpdateTextButton(button, buttonData);
            });
         /*   ViewModel.TextButton1.Subscribe(newText => _btnBuild1.GetComponentInChildren<TMP_Text>().text = newText);
            ViewModel.TextButton2.Subscribe(newText => _btnBuild2.GetComponentInChildren<TMP_Text>().text = newText);
            ViewModel.TextButton3.Subscribe(newText => _btnBuild3.GetComponentInChildren<TMP_Text>().text = newText);*/
        }

        private void UpdateTextButton(Button button, ButtonData buttonData)
        {
            button.transform.Find("Caption").GetComponentInChildren<TMP_Text>().text = buttonData.Caption;
            button.transform.Find("Level").GetComponentInChildren<TMP_Text>().text = buttonData.Level;
            button.transform.Find("Description").GetComponentInChildren<TMP_Text>().text = buttonData.Description;
            
            if (buttonData.PrehabImage != "")
            {
                button.transform.Find("Image").GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>($"Images/{buttonData.PrehabImage}");
            }
            
            //Debug.Log(button.name + " " + caption.name);
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
        }


        private void OnDisable()
        {
            _btnBuild1.onClick.RemoveListener(OnClickButtonBuild1);
            _btnBuild2.onClick.RemoveListener(OnClickButtonBuild2);
            _btnBuild3.onClick.RemoveListener(OnClickButtonBuild3);
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
    }
}