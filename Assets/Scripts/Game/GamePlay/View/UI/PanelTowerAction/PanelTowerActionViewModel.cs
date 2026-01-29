using System;
using DI;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.GameplayStates;
using Game.GamePlay.Services;
using Game.GamePlay.View.Frames;
using Game.GamePlay.View.Towers;
using Game.State;
using Game.State.Root;
using MVVM.UI;
using ObservableCollections;
using R3;
using UnityEngine;
using Random = UnityEngine.Random;

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

        public ReactiveProperty<bool> IsPlacement;


        private IDisposable _disposable;

        public PanelTowerActionViewModel(
            GameplayUIManager uiManager, 
            DIContainer container
        ) : base(container)
        {
            var d = Disposable.CreateBuilder();
            _uiManager = uiManager;

            _gameplayStateProxy = container.Resolve<IGameStateProvider>().GameplayState;
            
            _fsmGameplay = container.Resolve<FsmGameplay>();
            _frameService = container.Resolve<FrameService>();
            
            IsPlacement = new ReactiveProperty<bool>(true);

            var towerClick = container.Resolve<Subject<TowerViewModel>>();
            towerClick.Subscribe(towerViewModel =>
            {
                Debug.Log("Башня " + towerViewModel.UniqueId);
                IsPlacement.OnNext(towerViewModel.IsPlacement);
            });
            
            _disposable = d.Build();
        }

        public void RequestPlacement()
        {
       //     var card = ((FsmStateBuild)_fsmGameplay.Fsm.StateCurrent.Value).GetRewardCard();
     //       card.Direction = 2;
     //       card.Position = new Vector2Int(Random.Range(0, 5), Random.Range(0, 2));
   //         _fsmGameplay.Fsm.SetState<FsmStateBuildEnd>(card);
            //_fsmGameplay.Fsm.SetState<FsmStateGamePlay>();
        }
        
        public void RequestRemove()
        {
            //_fsmGameplay.Fsm.SetState<FsmStateBuildBegin>();
        }
        
        
        public override void Dispose()
        {
            _disposable.Dispose();
        }
        
    }
}