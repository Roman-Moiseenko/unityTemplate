using Game.MainMenu.View.ScreenShop;
using MVVM.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenu.View.ScreenClan
{
    public class ScreenClanBinder : WindowBinder<ScreenClanViewModel>
    {
        [SerializeField] private Button _btnGoToPlay;
        
        
        private void OnEnable()
        {
            _btnGoToPlay.onClick.AddListener(OnGoToPlayButtonClicked);
        }

        private void OnDisable()
        {
            _btnGoToPlay.onClick.RemoveListener(OnGoToPlayButtonClicked);
        }

        private void OnGoToPlayButtonClicked()
        {
            ViewModel.RequestGoToPlay();
        }
    }
}