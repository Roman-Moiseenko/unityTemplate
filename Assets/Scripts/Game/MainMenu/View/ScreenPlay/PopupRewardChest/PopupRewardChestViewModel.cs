using System.Collections.Generic;
using DI;
using Game.MainMenu.View.ScreenPlay.PopupFinishGameplay.PrefabBinders;
using Game.State.Inventory;
using Game.State.Inventory.Chests;
using Game.State.Maps.Rewards;
using MVVM.UI;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.MainMenu.View.ScreenPlay.PopupRewardChest
{
    public class PopupRewardChestViewModel : WindowViewModel
    {
        public override string Id => "PopupRewardChest";
        public override string Path => "MainMenu/ScreenPlay/Popups/";
        public List<ResourceRewardViewModel> RewardResources = new();
        public TypeChest TypeChest;

        public PopupRewardChestViewModel(
            TypeChest typeChest,
            List<RewardEntityData> rewards,
            DIContainer container)
        {
            TypeChest = typeChest;
            
            foreach (var reward in rewards)
            {
                var viewModel = new ResourceRewardViewModel
                {
                    InventoryType = reward.RewardType,
                    ConfigId = reward.ConfigId,
                    Amount = reward.Amount,
                };
                RewardResources.Add(viewModel);
            }
        }
    }
}