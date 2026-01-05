using DI;
using Game.GamePlay.Root;
using Game.GamePlay.Services;
using Game.State;
using MVVM.UI;
using R3;

namespace Game.GamePlay.View.UI.PopupLose
{
    public class PopupLoseViewModel : WindowViewModel
    {
        public override string Id => "PopupLose";
        
        public override string Path => "Gameplay/Popups/";
        
        private readonly Subject<GameplayExitParams> _exitSceneRequest;

        private readonly GameplayService _gameplayService;
        public ReactiveProperty<bool> ShowButtonAd = new();

        public PopupLoseViewModel(
            GameplayUIManager uiManager, 
            Subject<GameplayExitParams> exitSceneRequest,
            DIContainer container) : base(container)
        {
            _exitSceneRequest = exitSceneRequest;
            _gameplayService = container.Resolve<GameplayService>();

            var gameplayState = container.Resolve<IGameStateProvider>().GameplayState;
            ShowButtonAd.Value = gameplayState.Castle.CountResurrection.CurrentValue == 0;
        }

        public override void RequestClose()
        {
            base.RequestClose();
            _gameplayService.Lose();
        }

        public void RequestSpendCristal()
        {
            base.RequestClose();
            _gameplayService.RepairCristal();
        }

        public void RequestPlayAd()
        {
            base.RequestClose();
            _gameplayService.RepairAd();
        }
    }
}