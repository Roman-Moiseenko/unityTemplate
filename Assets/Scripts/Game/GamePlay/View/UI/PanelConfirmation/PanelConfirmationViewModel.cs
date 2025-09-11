using System;
using DI;
using Game.GamePlay.Commands.RewardCommand;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.GameplayStates;
using Game.GamePlay.Services;
using Game.GamePlay.View.Frames;
using Game.State;
using Game.State.Root;
using MVVM.CMD;
using MVVM.UI;
using ObservableCollections;
using R3;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.GamePlay.View.UI.PanelConfirmation
{
    public class PanelConfirmationViewModel : WindowViewModel
    {
        public override string Id => "PanelConfirmation";
        public override string Path => "Gameplay/Panels/";
        
        public readonly GameplayUIManager _uiManager;
        private readonly DIContainer _container;

        private readonly GameplayStateProxy _gameplayStateProxy;
        
        //Для теста
        private readonly FsmGameplay _fsmGameplay;
        private readonly FrameService _frameService;

        public ReactiveProperty<bool> IsEnable;
        public ReactiveProperty<bool> IsRotate;

        private IObservableCollection<FrameBlockViewModel> _frameBlocksView;
        private IDisposable _disposable;

        public PanelConfirmationViewModel(
            GameplayUIManager uiManager, 
            DIContainer container
        )
        {
            var d = Disposable.CreateBuilder();
            _uiManager = uiManager;
            _container = container;

            _gameplayStateProxy = container.Resolve<IGameStateProvider>().GameplayState;
            _fsmGameplay = container.Resolve<FsmGameplay>();
            _frameService = container.Resolve<FrameService>();
            _frameBlocksView = _frameService.ViewModels;

            IsEnable = new ReactiveProperty<bool>(true);
            IsRotate = new ReactiveProperty<bool>(true);
            
            _frameBlocksView.ObserveAdd().Subscribe(newValue =>
            {
                IsEnable.Value = newValue.Value.Enable.Value;
                newValue.Value.Enable.Subscribe(e => IsEnable.Value = e);
                IsRotate.Value = newValue.Value.IsRotate();
            }).AddTo(ref d);

            _frameBlocksView.ObserveRemove().Subscribe(_ =>
            {
                IsEnable.Value = true;
                IsRotate.Value = true;
            }).AddTo(ref d);
            _disposable = d.Build();
        }

        public void RequestConfirmation()
        {
            var card = ((FsmStateBuild)_fsmGameplay.Fsm.StateCurrent.Value).GetRewardCard();
            card.Direction = 2;
            card.Position = new Vector2Int(Random.Range(0, 5), Random.Range(0, 2));
            _fsmGameplay.Fsm.SetState<FsmStateBuildEnd>(card);
            _fsmGameplay.Fsm.SetState<FsmStateGamePlay>();
        }
        
        public void RequestCancel()
        {
            _fsmGameplay.Fsm.SetState<FsmStateBuildBegin>();
        }

        public void RequestRotate()
        {
            _frameService.RotateFrame();
        }
        
        public override void Dispose()
        {
            _disposable.Dispose();
        }
        
    }
}