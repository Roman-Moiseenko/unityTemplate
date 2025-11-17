using System;
using System.Collections.Generic;
using DI;
using Game.MainMenu.Root;
using Game.MainMenu.View.ScreenPlay.PopupFinishGameplay.PrefabBinders;
using Game.State;
using Game.State.Inventory;
using Game.State.Inventory.Chests;
using Game.State.Maps.Rewards;
using Game.State.Mergeable.ResourcesEntities;
using MVVM.UI;
using Newtonsoft.Json;
using ObservableCollections;
using UnityEngine;

namespace Game.MainMenu.View.ScreenPlay.PopupFinishGameplay
{
    public class PopupFinishGameplayViewModel : WindowViewModel
    {
        public readonly MainMenuEnterParams EnterParams;

        public TypeChest? RewardChest;
        public override string Id => "PopupFinishGameplay";
        public override string Path => "MainMenu/ScreenPlay/Popups/";
        public List<ResourceRewardViewModel> RewardResources = new();
        private readonly List<RewardEntityData> _rewards = new();

        public PopupFinishGameplayViewModel(MainMenuEnterParams enterParams, DIContainer container)
        {
            EnterParams = enterParams;
            var gameState = container.Resolve<IGameStateProvider>().GameState;

            _rewards.Add(new RewardEntityData
            {
                RewardType = InventoryType.SoftCurrency,
                ConfigId = "Currency",
                Amount = enterParams.SoftCurrency,
            });

            foreach (var rewardCard in enterParams.RewardCards)
            {
                var element = _rewards.Find(v =>
                    v.RewardType == rewardCard.RewardType &&
                    v.ConfigId == rewardCard.ConfigId);
                if (element != null)
                {
                    element.Amount += rewardCard.Amount;
                }
                else
                {
                    _rewards.Add(rewardCard);
                }
            }

            foreach (var rewardEntity in enterParams.RewardOnWave)
            {
                _rewards.Add(rewardEntity);
            }

            foreach (var reward in _rewards)
            {
                var viewModel = new ResourceRewardViewModel
                {
                    InventoryType = reward.RewardType,
                    ConfigId = (reward.ConfigId == "") ? "UnKnow" : reward.ConfigId,
                    Amount = reward.Amount,
                };
                RewardResources.Add(viewModel);
            }

            RewardChest = enterParams.TypeChest;
        }

        private void RewardLoad(List<RewardEntityData> list)
        {
        }
    }
}