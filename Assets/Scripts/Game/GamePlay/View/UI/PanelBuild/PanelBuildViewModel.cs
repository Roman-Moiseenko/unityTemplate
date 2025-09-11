using System;
using System.Collections.Generic;
using DI;
using Game.Common;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.GameplayStates;
using Game.GamePlay.Services;
using Game.GameRoot.Services;
using Game.Settings;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State;
using Game.State.Gameplay;
using Game.State.Root;
using MVVM.FSM;
using MVVM.UI;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using UnityEngine;
using Object = System.Object;

namespace Game.GamePlay.View.UI.PanelBuild
{
    public class PanelBuildViewModel : WindowViewModel
    {
        public override string Id => "PanelBuild";
        public override string Path => "Gameplay/Panels/BuildCards/";
        
        public ReactiveProperty<int> UpdateCards;
        public ObservableDictionary<int, ButtonData> ButtonCards = new();

        private readonly ResourceService _resourceService;
        private readonly RewardProgressService _rewardService;

        private RewardsProgress _rewards;
        private readonly GameplayStateProxy _gameplayState;
        private readonly IDisposable _disposable;
        public ObservableDictionary<int, RewardCardData> RewardsCards = new();
        public List<TowerSettings> AllTowerConfig { get; private set; }
        public readonly ObservableDictionary<string, int> Levels;
        public Dictionary<int, CardViewModel> CardViewModels = new();
        
        public PanelBuildViewModel(DIContainer container)
        {
            var d = Disposable.CreateBuilder();
            var gameplayState = container.Resolve<IGameStateProvider>().GameplayState;
            var towerService  = container.Resolve<TowersService>();
            var fsmGameplay = container.Resolve<FsmGameplay>();
            var gameSettings = container.Resolve<ISettingsProvider>().GameSettings;
            
            UpdateCards = gameplayState.UpdateCards;
            _gameplayState = container.Resolve<IGameStateProvider>().GameplayState;
            _resourceService = container.Resolve<ResourceService>();
            _rewardService = container.Resolve<RewardProgressService>();
            Levels = towerService.Levels;
            
            CardViewModels.Add(1, new CardViewModel(gameSettings, fsmGameplay, towerService));
            CardViewModels.Add(2, new CardViewModel(gameSettings, fsmGameplay, towerService));
            CardViewModels.Add(3, new CardViewModel(gameSettings, fsmGameplay, towerService));
            
            AllTowerConfig = container.Resolve<ISettingsProvider>().GameSettings.TowersSettings.AllTowers;
            fsmGameplay.Fsm.StateCurrent
                .Where(newState => newState.GetType() == typeof(FsmStateBuildBegin))
                .Subscribe(_ =>
                {
                    if (fsmGameplay.Fsm.PreviousState.GetType() != typeof(FsmStateBuild))
                    {
                        LoadRewardsToCards();                        
                    }
                }).AddTo(ref d);
            _disposable = d.Build();
        }

        private void LoadRewardsToCards()
        {
            RewardsCards.Clear();
            var start = _gameplayState.Progress.CurrentValue == 0;
            _rewards = start ? _rewardService.StartRewardProgress() : _rewardService.GenerateRewardProgress();

            for (var i = 1; i <= 3; i++)
            {
                CardViewModels[i].UpdateRewardInfo(_rewards.Cards[i]);
            }
        }
        
        /**
         * Обновить награду, увеличить стоимость
         */
        public void OnUpdateCard()
        {
            if (!_resourceService.SpendHardCurrency(UpdateCards.CurrentValue * AppConstants.COST_UPDATE_BUILD))
                return;
            
            UpdateCards.Value++;
            LoadRewardsToCards();
        }

        public override void Dispose()
        {
            _disposable.Dispose();
        }
    }
}