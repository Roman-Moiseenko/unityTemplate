using System.Collections.Generic;
using DI;
using Game.MainMenu.Root;
using Game.MainMenu.View.ScreenPlay.PopupFinishGameplay.PrefabBinders;
using Game.State;
using Game.State.Inventory;
using Game.State.Inventory.Chests;
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
        private readonly Dictionary<InventoryType, Dictionary<string, long>> _rewards = new();

        public PopupFinishGameplayViewModel(MainMenuEnterParams enterParams, DIContainer container)
        {
            EnterParams = enterParams;
            var gameState = container.Resolve<IGameStateProvider>().GameState;

            var gold = new Dictionary<string, long>();
            gold.Add("Currency", enterParams.SoftCurrency);
            
            _rewards.Add(InventoryType.Other, gold);
            
            foreach (var rewardCard in enterParams.RewardCards)
            {
                if (_rewards.TryGetValue(rewardCard.RewardType, out var configCounts))
                {
                    if (configCounts.TryGetValue(rewardCard.ConfigId, out var value))
                    {
                        configCounts[rewardCard.ConfigId]++;
                    }
                    else
                    {
                        configCounts.Add(rewardCard.ConfigId, 1);
                    }
                }
                else
                {
                    var config = new Dictionary<string, long>();
                    config.Add(rewardCard.ConfigId, 1);
                    _rewards.Add(rewardCard.RewardType, config);
                }
            }

            foreach (var (type, value) in _rewards)
            {
                foreach (var (config, amount) in value)
                {
                    var viewModel = new ResourceRewardViewModel
                    {
                        InventoryType = type,
                        ConfigId = config,
                        Amount = amount,
                    };
                    RewardResources.Add(viewModel);
                }
            }
            
            RewardChest = enterParams.TypeChest;
        }
    }
}