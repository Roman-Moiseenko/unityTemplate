using System;
using System.Collections.Generic;
using System.Linq;
using Game.MainMenu.View.MainScreen.BotomMenu;
using MVVM.UI;
using R3;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.MainMenu.View.MainScreen
{
    public class MainScreenBinder : WindowBinder<MainScreenViewModel>
    {
        //Кнопки боттом-меню
        [SerializeField] private List<Button> bottomButtons;
        private readonly Dictionary<string, ButtonBinder> _buttonBinders = new();

        //Кнопки топ-меню
        [SerializeField] private Button btnAccount;

        private const int HeightBottomMenu = 200;

        private IDisposable _disposable;
        private void Awake()
        {
            var d = Disposable.CreateBuilder();
            foreach (var button in bottomButtons)
            {
                var buttonBinder = button.GetComponent<ButtonBinder>();
                buttonBinder.Bind();
                _buttonBinders.Add(button.name, buttonBinder);
            }
            transform.Find("BottomMenu").GetComponent<RectTransform>().sizeDelta = new Vector2(0, HeightBottomMenu);
            _disposable = d.Build();
        }

        protected override void OnBind(MainScreenViewModel viewModel)
        {

        }

        private void ClickUpdateButton(string nameButton)
        {
            var baseWidth = Mathf.FloorToInt(Screen.width / 5.5f);
            var i = 0;
            var delta = 0;
            var sizeDelta = new Vector2(0, HeightBottomMenu);
            var position = new Vector3(0, HeightBottomMenu / 2, 0);
            foreach (var btnBinder in _buttonBinders.Select(buttonKey => buttonKey.Value))
            {
                if (!btnBinder.HasName(nameButton))
                {
                    sizeDelta.x = baseWidth;
                    position.x = i * baseWidth + delta * 2;
                    btnBinder.UnClick();
                }
                else
                {
                    delta = baseWidth / 4;
                    sizeDelta.x = baseWidth * 1.5f;
                    position.x = i * baseWidth;
                    btnBinder.Click();
                }
                
                btnBinder.Resize(sizeDelta, position);
                i++;
            }
        }

        private void OnEnable()
        {
            ClickUpdateButton("Play");
        }
        
        public void OnResearch()
        {
            ClickUpdateButton("Research");
            ViewModel.OpenResearch();
        }

        public void OnClan()
        {
            ClickUpdateButton("Clan");
            ViewModel.OpenClan();
        }

        public void OnPlay()
        {
            ClickUpdateButton("Play");
            ViewModel.OpenPlay();
        }

        public void OnInventory()
        {
            ClickUpdateButton("Inventory");
            ViewModel.OpenInventory();
        }

        public void OnShop()
        {
            ClickUpdateButton("Shop");
            ViewModel.OpenShop();
        }


        private void OnDestroy()
        {
            _disposable.Dispose();
        }
    }
}