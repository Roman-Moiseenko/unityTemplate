using System;
using DI;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.HeroStates;
using Game.GamePlay.Services;
using MVVM.UI;
using ObservableCollections;
using R3;

namespace Game.GamePlay.View.UI.PanelHeroPlacement
{
    public class PanelHeroPlacementViewModel : PanelViewModel
    {
        private readonly FsmHero _fsmHero;
        public override string Id => "PanelHeroPlacement";
        public override string Path => "Gameplay/Panels/";
        
        public ReactiveProperty<bool> IsEnable;

        private IDisposable _temp;
        //public IDisposable _temp;

        public PanelHeroPlacementViewModel(GameplayUIManager uiManager, DIContainer container) : base(container)
        {
            _fsmHero = container.Resolve<FsmHero>();
            IsEnable = new ReactiveProperty<bool>(true);

            var frameHeroService = container.Resolve<FrameHeroService>();
            //var frameModelViews = frameHeroService.ViewModels;
            
            frameHeroService.ViewModel.Subscribe(model =>
            {
                if (model != null)
                {
                    _temp = model.Enable.Subscribe(v => IsEnable.Value = v);
                } 
                else
                {
                    _temp?.Dispose();
                    _temp = null;
                }
                
            }).AddTo(ref _disposables);
        }

        public void RequestConfirmation()
        {
            _fsmHero.Fsm.SetState<FsmHeroPlacementEnd>();
            RequestClose();
            _fsmHero.Fsm.SetState<FsmHeroAwait>();
        }

        public void RequestCancel()
        {
            _fsmHero.Fsm.SetState<FsmHeroUnSelected>();
            RequestClose();
        }
    }
}