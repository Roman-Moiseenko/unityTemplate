using Game.MainMenu.View.ScreenShop;
using MVVM.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenu.View.ScreenClan
{
    public class ScreenClanBinder : WindowBinder<ScreenClanViewModel>
    {
        //   [SerializeField] private Button _btnGoToPlay;

        protected override void OnBind(ScreenClanViewModel viewModel)
        {
            base.OnBind(viewModel);
           // Debug.Log("ScreenClanBinder");
        }

        
        private void OnEnable()
        {
    //        _btnGoToPlay.onClick.AddListener(OnGoToPlayButtonClicked);
        }

        private void OnDisable()
        {
    //        _btnGoToPlay.onClick.RemoveListener(OnGoToPlayButtonClicked);
        }

        private void OnGoToPlayButtonClicked()
        {
            ViewModel.RequestGoToPlay();
        }
    }
}