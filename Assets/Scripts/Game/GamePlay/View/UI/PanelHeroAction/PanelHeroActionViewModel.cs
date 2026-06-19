using DI;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.HeroStates;
using MVVM.UI;
using R3;

namespace Game.GamePlay.View.UI.PanelHeroAction
{
    public class PanelHeroActionViewModel : PanelViewModel
    {
        public override string Id => "PanelHeroAction";
        public override string Path => "Gameplay/Panels/";
        
        public readonly GameplayUIManager _uiManager;

        //private readonly GameplayStateProxy _gameplayStateProxy;
        
        //Для теста
        //private readonly FsmGameplay _fsmGameplay;
        //private readonly FrameService _frameService;

        public readonly ReactiveProperty<bool> IsPlacement = new(false);
        private readonly FsmHero _fsmHero;

        public PanelHeroActionViewModel(
            GameplayUIManager uiManager, 
            DIContainer container
        ) : base(container)
        {
            _uiManager = uiManager;
            _fsmHero = container.Resolve<FsmHero>();
            //_gameplayStateProxy = container.Resolve<IGameStateProvider>().GameplayState;
            //var heroesService = container.Resolve<HeroesService>();
            
            //_fsmGameplay = container.Resolve<FsmGameplay>();

        }

        public void RequestPlacement()
        {
            _fsmHero.Fsm.SetState<FsmHeroPlacement>();
        }
        
    }
}