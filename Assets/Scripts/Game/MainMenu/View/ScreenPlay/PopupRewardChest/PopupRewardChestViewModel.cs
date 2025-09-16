using System.Collections.Generic;
using DI;
using Game.MainMenu.View.ScreenPlay.PopupFinishGameplay.PrefabBinders;
using Game.State.Inventory;
using Game.State.Inventory.Chests;
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
            Dictionary<InventoryType, Dictionary<string, int>> rewards, 
            DIContainer container)
        {
            TypeChest = typeChest;
            Debug.Log(JsonConvert.SerializeObject(rewards, Formatting.Indented));
            foreach (var (type, value) in rewards)
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
        }
    }
}