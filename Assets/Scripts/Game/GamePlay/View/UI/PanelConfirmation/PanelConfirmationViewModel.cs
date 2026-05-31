using System;
using DI;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.GameplayStates;
using Game.GamePlay.Services;
using Game.State;
using Game.State.Gameplay;
using Game.State.Root;
using MVVM.UI;
using ObservableCollections;
using R3;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.GamePlay.View.UI.PanelConfirmation
{
    public class PanelConfirmationViewModel : PanelViewModel
    {
        public override string Id => "PanelConfirmation";
        public override string Path => "Gameplay/Panels/";

        public readonly GameplayUIManager _uiManager;

        private readonly GameplayStateProxy _gameplayStateProxy;

        //Для теста
        private readonly FsmGameplay _fsmGameplay;
        private readonly FrameService _frameService;

        public ReactiveProperty<bool> IsEnable;
        public ReactiveProperty<bool> IsConfirmation;
        public ReactiveProperty<bool> IsRotate;


        public PanelConfirmationViewModel(
            GameplayUIManager uiManager,
            DIContainer container
        ) : base(container)
        {
            _uiManager = uiManager;

            _gameplayStateProxy = container.Resolve<IGameStateProvider>().GameplayState;
            _fsmGameplay = container.Resolve<FsmGameplay>();
            _frameService = container.Resolve<FrameService>();

            IsEnable = new ReactiveProperty<bool>(false);
            IsRotate = new ReactiveProperty<bool>(false);
            IsConfirmation = new ReactiveProperty<bool>(false);

            _fsmGameplay.Fsm.StateCurrent.Subscribe(v =>
            {
                if (v.GetType() == typeof(FsmStateBuildBegin))
                {
                    IsEnable.Value = false;
                    IsRotate.Value = false;
                    IsConfirmation.Value = false;
                }
            }).AddTo(ref _disposables);
            _frameService.CurrentFrame
                .Subscribe(viewModel =>
                {
                    if (viewModel != null)
                    {
                        IsConfirmation.Value = true;
                        IsEnable.Value = viewModel.Enable.Value;
                        viewModel.Enable.Subscribe(ev => IsEnable.Value = ev);
                        IsRotate.Value = viewModel.IsRotate();
                    }
                    else
                    {
                        IsEnable.Value = true;
                        IsRotate.Value = false;
                    }
                })
                .AddTo(ref _disposables);
        }

        public void RequestConfirmation()
        {
            var card = ((FsmStateBuild)_fsmGameplay.Fsm.StateCurrent.Value).GetRewardCard();
            card.Direction = 2;
            card.Position = new Vector2Int(Random.Range(0, 5), Random.Range(0, 2));
            _fsmGameplay.Fsm.SetState<FsmStateBuildEnd>(card);
            //_fsmGameplay.Fsm.SetState<FsmStateGamePlay>();
        }

        public void RequestCancel()
        {
            _fsmGameplay.Fsm.SetState<FsmStateBuildBegin>();
        }

        public void RequestRotate()
        {
            _frameService.RotateFrame();
        }
    }
}