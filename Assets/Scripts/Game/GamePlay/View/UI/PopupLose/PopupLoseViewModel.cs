using DI;
using Game.GamePlay.Root;
using Game.GamePlay.Services;
using Game.State;
using Game.State.Root;
using MVVM.UI;
using R3;

namespace Game.GamePlay.View.UI.PopupLose
{
    public class PopupLoseViewModel : WindowViewModel
    {
        public override string Id => "PopupLose";
        
        public override string Path => "Gameplay/Popups/";
        
        private readonly Subject<GameplayExitParams> _exitSceneRequest;
        private readonly DIContainer _container;

        private readonly GameplayStateProxy _gameplayState;
        private readonly GameplayService _gameplayService;

        public PopupLoseViewModel(
            GameplayUIManager uiManager, 
            Subject<GameplayExitParams> exitSceneRequest,
            DIContainer container)
        {
            _exitSceneRequest = exitSceneRequest;
            _container = container;
            _gameplayService = _container.Resolve<GameplayService>();

            _gameplayState = container.Resolve<IGameStateProvider>().GameplayState;
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