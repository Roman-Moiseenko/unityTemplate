using System;
using DI;
using Game.GamePlay.Commands.RewardCommand;
using Game.State;
using Game.State.Root;
using MVVM.CMD;
using MVVM.UI;
using R3;

namespace Game.GamePlay.View.UI.PanelActions
{
    public class PanelActionsViewModel : WindowViewModel
    {
        public override string Id => "PanelActions";
        public override string Path => "Gameplay/Panels/";
        
        public readonly GameplayUIManager _uiManager;
        private readonly DIContainer _container;
        
        public readonly ReactiveProperty<int> CurrentSpeed;
        private readonly GameplayStateProxy _gameplayStateProxy;
        
        private IDisposable _disposable;
        
        public PanelActionsViewModel(
            GameplayUIManager uiManager, 
            DIContainer container
        )
        {
            var d = Disposable.CreateBuilder();
            _uiManager = uiManager;
            _container = container;

            _gameplayStateProxy = container.Resolve<IGameStateProvider>().GameplayState;
  
            CurrentSpeed = _gameplayStateProxy.GameSpeed;
            _disposable = d.Build();
        }
        public void RequestGameSpeed()
        {
            _gameplayStateProxy.SetNextSpeed();
        }

        public void RequestToProgressAdd()
        {
            var cmd = _container.Resolve<ICommandProcessor>();
            var command = new CommandRewardKillMob(25, 1);
            cmd.Process(command);
        }
        
        public override void Dispose()
        {
            _disposable.Dispose();
        }
    }
}