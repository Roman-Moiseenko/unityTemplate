using MVVM.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenu.View.ScreenPlay
{
    public class ScreenPlayBinder : WindowBinder<ScreenPlayViewModel>
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