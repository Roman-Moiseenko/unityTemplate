using DI;
using Game.GamePlay.Commands.RewardCommand;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.States;
using Game.GamePlay.Services;
using Game.MainMenu.Services;
using Game.State;
using Game.State.GameResources;
using Game.State.Root;
using MVVM.CMD;
using MVVM.UI;
using R3;

namespace Game.GamePlay.View.UI.PanelActions
{
    public class PanelActionsViewModel : WindowViewModel
    {
        public override string Id => "PanelActions";
        public override string Path => "Gameplay/";
        
        public readonly GameplayUIManager _uiManager;
        private readonly DIContainer _container;
        
        public readonly ReactiveProperty<int> CurrentSpeed;
        private readonly GameplayStateProxy _gameplayStateProxy;
        
        //Для теста
        private readonly FsmGameplay _fsmGameplay;
        public PanelActionsViewModel(
            GameplayUIManager uiManager, 
            DIContainer container
        )
        {
            _uiManager = uiManager;
            _container = container;

            _gameplayStateProxy = container.Resolve<IGameStateProvider>().GameplayState;
            _fsmGameplay = container.Resolve<FsmGameplay>();
            CurrentSpeed = _gameplayStateProxy.GameSpeed;
            
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
    }
}