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
        
        private readonly GameplayState _gameplayState;
        
        //Для теста
        private readonly FsmGameplay _fsmGameplay;
        private ResourcesService _resourcesService;
        public PanelActionsViewModel(
            GameplayUIManager uiManager, 
            DIContainer container
        )
        {
            _uiManager = uiManager;
            
            _gameplayState = container.Resolve<IGameStateProvider>().GameState.GameplayState;
            _fsmGameplay = container.Resolve<FsmGameplay>();
            CurrentSpeed = _gameplayState.GetCurrentSpeed();
            
            //Для теста
            _resourcesService = container.Resolve<ResourcesService>();
        }
        public int RequestGameSpeed()
        {
            return _gameplayState.SetNextSpeed();
        }

        public void RequestToProgressAdd()
        {
            _gameplayState.Progress.Value += 50;
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