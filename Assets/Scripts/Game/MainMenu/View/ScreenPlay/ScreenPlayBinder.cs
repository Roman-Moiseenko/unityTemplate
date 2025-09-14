using Game.MainMenu.View.ScreenPlay.Chests;
using MVVM.UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.MainMenu.View.ScreenPlay
{
    public class ScreenPlayBinder : WindowBinder<ScreenPlayViewModel>
    {
        [SerializeField] private Button btnInfinityBattle;
        [SerializeField] private Button _btnResumeGame;
        [SerializeField] private ChestsBinder chests;


        protected override void OnBind(ScreenPlayViewModel viewModel)
        {
            base.OnBind(viewModel);
            chests.Bind(viewModel.ChestsViewModel);
            
        }

        private void OnEnable()
        {
            btnInfinityBattle.onClick.AddListener(OnResumeInfinityBattleClicked);
            _btnResumeGame.onClick.AddListener(OnResumeGameButtonClicked);
        }

        private void OnDisable()
        {
            btnInfinityBattle.onClick.RemoveListener(OnResumeInfinityBattleClicked);
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
        
        
        private void OnResumeInfinityBattleClicked()
        {
            if (ViewModel.RequestInfinityGame())
            {
                //TODO Попап с ошибкой . Не назначены карты, скилы или герой, или не потратили что то еще
            }

            
        }
    }
}