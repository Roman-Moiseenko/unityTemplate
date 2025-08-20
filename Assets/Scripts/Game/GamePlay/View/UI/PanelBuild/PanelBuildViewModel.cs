using System;
using DI;
using Game.Common;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.States;
using Game.GamePlay.Services;
using Game.GameRoot.Services;
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
        public override string Path => "Gameplay/Panels/";
        
        public ReactiveProperty<int> UpdateCards;

        public ObservableDictionary<int, ButtonData> ButtonCards = new();
        private readonly FsmGameplay _fsmGameplay;
        
        private readonly ResourceService _resourceService;
        private readonly RewardProgressService _rewardService;

        private RewardsProgress _rewards;
        private readonly GameplayStateProxy _gameplayState;
        private readonly IDisposable _disposable;

        public PanelBuildViewModel(DIContainer container)
        {
            var d = Disposable.CreateBuilder();
            var gameplayState = container.Resolve<IGameStateProvider>().GameplayState;
            UpdateCards = gameplayState.UpdateCards;
            _gameplayState = container.Resolve<IGameStateProvider>().GameplayState;

            _resourceService = container.Resolve<ResourceService>();
            _rewardService = container.Resolve<RewardProgressService>();
            _fsmGameplay = container.Resolve<FsmGameplay>();
            _fsmGameplay.Fsm.StateCurrent
                .Where(newState => newState.GetType() == typeof(FsmStateBuildBegin))
                .Subscribe(_ => LoadRewardsToCards()).AddTo(ref d);
            _disposable = d.Build();
        }

        private void LoadRewardsToCards()
        {
            ButtonCards.Clear();
            var start = _gameplayState.Progress.CurrentValue == 0;
            _rewards = start ? _rewardService.StartRewardProgress() : _rewardService.GenerateRewardProgress();
            
            foreach (var rewardsCard in _rewards.Cards)
            {
                ButtonCards.Add(rewardsCard.Key, GetTextRewardButton(rewardsCard.Value));
            }
        }

        private ButtonData GetTextRewardButton(RewardCardData cardData)
        {
            var buttonData = new ButtonData();
            switch (cardData.RewardType)
            {
                case RewardType.Tower:
                    buttonData.Caption = "Построить башню";
                    buttonData.PrehabImage = "Towers/" + cardData.ConfigId + "/Level_" + cardData.RewardLevel;
                    buttonData.Level = cardData.RewardLevel.ToString();
                    buttonData.Description = "Башня " + cardData.Name;
                    break;
                case RewardType.Ground:
                    buttonData.Caption = "Построить участок";
                    buttonData.PrehabImage = "Ground";

                    break;
                case RewardType.Road:
                    buttonData.Caption = "Построить дорогу";
                    buttonData.PrehabImage = "Roads/" + cardData.ConfigId;
                    buttonData.Description = cardData.Description;
                    break;
                case RewardType.TowerLevelUp:
                    buttonData.Caption = "Улучшить башню";
                    buttonData.PrehabImage = "Towers/" + cardData.ConfigId + "/Level_" + cardData.RewardLevel;
                    buttonData.Level = cardData.RewardLevel.ToString() + " +1";
                    buttonData.Description = cardData.Description;
                    break;
                case RewardType.SkillLevelUp:
                    buttonData.Caption = "Улучшить навык";
                    break;
                case RewardType.HeroLevelUp:
                    buttonData.Caption = "Улучшить героя";
                    break;
                case RewardType.TowerMove:
                    buttonData.Caption = "Передвинуть башню";
                    break;
                case RewardType.TowerReplace:
                    buttonData.Caption = "Заменить башни";
                    break;
                default: throw new Exception("Не известное значение");
            }

            return buttonData;
        }

        public void OnBuild1()
        {
            BuildStateProgress(_rewards.Cards[1]); //Строим или применяем навык
        }

        public void OnBuild2()
        {
            BuildStateProgress(_rewards.Cards[2]); //Строим или применяем навык
        }

        public void OnBuild3()
        {
            BuildStateProgress(_rewards.Cards[3]); //Строим или применяем навык
        }

        private void BuildStateProgress(RewardCardData cardData)
        {
            if (cardData.IsBuild())
            {
                _fsmGameplay.Fsm.SetState<FsmStateBuild>(cardData);
            }
            else
            {
                _fsmGameplay.Fsm.SetState<FsmStateBuildEnd>(cardData);
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