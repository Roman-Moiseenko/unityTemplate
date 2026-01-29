using System;
using DI;
using Game.GamePlay.Commands.RewardCommand;
using Game.GamePlay.Fsm;
using Game.State;
using Game.State.Root;
using MVVM.CMD;
using MVVM.UI;
using R3;

namespace Game.GamePlay.View.UI.PanelActions
{
    public class PanelActionsViewModel : PanelViewModel
    {
        public override string Id => "PanelActions";
        public override string Path => "Gameplay/Panels/";
        
        public readonly GameplayUIManager _uiManager;
        
        //public readonly ReactiveProperty<int> CurrentSpeed;
        private readonly GameplayStateProxy _gameplayStateProxy;
        
        private IDisposable _disposable;
        
        public PanelActionsViewModel(
            GameplayUIManager uiManager, 
            DIContainer container
        ) : base(container)
        {
            var d = Disposable.CreateBuilder();
            IsShow = true; //По-умолчанию показываем
            _uiManager = uiManager;
            _gameplayStateProxy = container.Resolve<IGameStateProvider>().GameplayState;
            var fsmGameplay = container.Resolve<FsmGameplay>();
            fsmGameplay.Fsm.StateCurrent.Subscribe();
            _disposable = d.Build();
        }
        public void RequestGameSpeed()
        {
            _gameplayStateProxy.SetNextSpeed();
        }

        public void RequestToProgressAdd()
        {
            var cmd = Container.Resolve<ICommandProcessor>();
            var command = new CommandRewardKillMob(25, 1);
            cmd.Process(command);
        }
        
        public override void Dispose()
        {
            _disposable.Dispose();
        }

        public float GetCurrentSpeed()
        {
            return _gameplayStateProxy.GetCurrentSpeed();
        }
    }
}