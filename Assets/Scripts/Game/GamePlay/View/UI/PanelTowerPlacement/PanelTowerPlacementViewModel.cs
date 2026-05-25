using System;
using System.Collections.Generic;
using DI;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.TowerStates;
using Game.GamePlay.Services;
using MVVM.UI;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.UI.PanelTowerPlacement
{
    public class PanelTowerPlacementViewModel : PanelViewModel
    {
        private readonly FsmTower _fsmTower;
        public override string Id => "PanelTowerPlacement";
        public override string Path => "Gameplay/Panels/";
        
        public ReactiveProperty<bool> IsEnable;

        public PanelTowerPlacementViewModel(GameplayUIManager uiManager, DIContainer container) : base(container)
        {
            _fsmTower = container.Resolve<FsmTower>();
            IsEnable = new ReactiveProperty<bool>(true);

            var framePlacementService = container.Resolve<FramePlacementService>();
            var frameModelViews = framePlacementService.ViewModels;
            
            frameModelViews.ObserveAdd().Subscribe(e =>
            {
                var viewModel = e.Value;
                viewModel.Enable.Subscribe(v => IsEnable.Value = v)
                    .AddTo(ref _disposables);
            });

            frameModelViews.ObserveRemove().Subscribe(v =>
            {
                _disposables.Dispose();
            });
        }

        public void RequestConfirmation()
        {
            _fsmTower.Fsm.SetState<FsmTowerPlacementEnd>();
            RequestClose();
            _fsmTower.Fsm.SetState<FsmTowerNone>();
        }

        public void RequestCancel()
        {
            _fsmTower.Fsm.SetState<FsmTowerNone>();
            RequestClose();
        }
    }
}