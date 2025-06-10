using DI;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.States;
using Game.GamePlay.Services;
using Game.State;
using Game.State.GameResources;
using Game.State.Root;
using MVVM.UI;
using R3;

namespace Game.GamePlay.View.UI.PanelActions
{
    public class PanelActionsViewModel : WindowViewModel
    {
        public override string Id => "PanelActions";
        public override string Path => "Gameplay/";
        
        public readonly int CurrentSpeed;
        public readonly GameplayUIManager _uiManager;
        
        private readonly GameplayStateProxy _gameplayStateProxy;
        
        //Для теста
        private readonly FsmGameplay _fsmGameplay;
        private ResourcesService _resourcesService;
        public PanelActionsViewModel(
            GameplayUIManager uiManager, 
            DIContainer container
        )
        {
            _uiManager = uiManager;
            
            _gameplayStateProxy = container.Resolve<IGameStateProvider>().GameState.GameplayStateProxy;
            _fsmGameplay = container.Resolve<FsmGameplay>();
            CurrentSpeed = _gameplayStateProxy.GetCurrentSpeed();
            
            //Для теста
            _resourcesService = container.Resolve<ResourcesService>();
        }
        public int RequestGameSpeed()
        {
            return _gameplayStateProxy.SetNextSpeed();
        }

        public void RequestToProgressAdd()
        {
            _gameplayStateProxy.Progress.Value += 50;
            //_fsmGameplay.Fsm.SetState<FsmStateBuildBegin>();
        }

        public void RequestToSoftCurrencyAdd()
        {
            _resourcesService.AddResource(ResourceType.SoftCurrency, 50);
            
        }

        public void RequestToHardCurrencyAdd()
        {
            _resourcesService.AddResource(ResourceType.HardCurrency, 50);
        }
    }
}