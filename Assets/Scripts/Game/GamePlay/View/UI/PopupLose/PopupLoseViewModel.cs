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
        
        public PopupLoseViewModel(
            GameplayUIManager uiManager, 
            Subject<GameplayExitParams> exitSceneRequest,
            DIContainer container)
        {
            _exitSceneRequest = exitSceneRequest;
            _container = container;

            _gameplayState = container.Resolve<IGameStateProvider>().GameplayState;
        }

        public override void RequestClose()
        {
            base.RequestClose();
            _container.Resolve<GameplayService>().Lose();
        }

        public void RequestSpendCristal()
        {
            base.RequestClose();
            throw new System.NotImplementedException();
        }

        public void RequestPlayAd()
        {
            base.RequestClose();
            throw new System.NotImplementedException();
        }
    }
}