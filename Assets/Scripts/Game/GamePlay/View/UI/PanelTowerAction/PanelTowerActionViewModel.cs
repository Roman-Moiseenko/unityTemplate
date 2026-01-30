using System;
using DI;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.TowerStates;
using Game.GamePlay.Services;
using Game.GamePlay.View.Towers;
using Game.State;
using Game.State.Root;
using MVVM.UI;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.UI.PanelTowerAction
{
    public class PanelTowerActionViewModel : PanelViewModel
    {
        public override string Id => "PanelTowerAction";
        public override string Path => "Gameplay/Panels/";
        
        public readonly GameplayUIManager _uiManager;

        private readonly GameplayStateProxy _gameplayStateProxy;
        
        //Для теста
        private readonly FsmGameplay _fsmGameplay;
        private readonly FrameService _frameService;

        public ReactiveProperty<bool> IsPlacement = new(false);


        private IDisposable _disposable;
        private readonly FsmTower _fsmTower;

        public PanelTowerActionViewModel(
            GameplayUIManager uiManager, 
            DIContainer container
        ) : base(container)
        {
            var d = Disposable.CreateBuilder();
            _uiManager = uiManager;

            _gameplayStateProxy = container.Resolve<IGameStateProvider>().GameplayState;
            
            _fsmGameplay = container.Resolve<FsmGameplay>();
            _fsmTower = container.Resolve<FsmTower>();
            _fsmTower.Fsm.StateCurrent.Subscribe(state =>
            {
                if (state.GetType() == typeof(FsmTowerSelected))
                {
                    var towerViewModel = _fsmTower.GetTowerViewModel();
                    IsPlacement.OnNext(towerViewModel.IsPlacement);    
                }
            });
            _frameService = container.Resolve<FrameService>();
            
            
            _disposable = d.Build();
        }

        public void RequestPlacement()
        {
            _fsmTower.Fsm.SetState<FsmTowerPlacement>();
        }
        
        public void RequestRemove()
        {
            _fsmTower.Fsm.SetState<FsmTowerDelete>();
        }
        
        
        public override void Dispose()
        {
            _disposable.Dispose();
        }
        
    }
}