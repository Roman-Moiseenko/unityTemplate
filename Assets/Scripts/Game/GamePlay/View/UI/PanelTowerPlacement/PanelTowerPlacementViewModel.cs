using DI;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.TowerStates;
using Game.GamePlay.Services;
using MVVM.UI;
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

            _fsmTower.Fsm.StateCurrent
                .Subscribe(state =>
            {
                Debug.Log(" PanelTowerPlacementViewModel ");
                if (state.GetType() == typeof(FsmTowerPlacement))
                {
                    
                    IsEnable = _fsmTower.GetTowerViewModel().IsConfirmationState;
                    Debug.Log(" FsmTowerPlacement " + IsEnable.Value + " " + _fsmTower.GetTowerViewModel().UniqueId);

                }

                if (state.GetType() == typeof(FsmTowerNone))
                {
                    IsEnable = new ReactiveProperty<bool>(true);
                }
                
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
            _fsmTower.Fsm.SetState<FsmTowerSelected>();
            RequestClose();
        }
    }
}