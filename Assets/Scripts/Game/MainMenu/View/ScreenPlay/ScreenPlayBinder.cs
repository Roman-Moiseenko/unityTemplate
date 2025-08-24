using MVVM.UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.MainMenu.View.ScreenPlay
{
    public class ScreenPlayBinder : WindowBinder<ScreenPlayViewModel>
    {
        [SerializeField] private Button _btnBeginGame;
        [SerializeField] private Button _btnResumeGame;
        
        
        private void OnEnable()
        {
            _btnBeginGame.onClick.AddListener(OnBeginGameButtonClicked);
            _btnResumeGame.onClick.AddListener(OnResumeGameButtonClicked);
        }

        private void OnDisable()
        {
            _btnBeginGame.onClick.RemoveListener(OnBeginGameButtonClicked);
            _btnResumeGame.onClick.RemoveListener(OnResumeGameButtonClicked);
        }

        private void OnBeginGameButtonClicked()
        {
            ViewModel.RequestBeginGame();
        }

        private void OnResumeGameButtonClicked()
        {
            ViewModel.RequestResumeGame();
        }
        
    }
}