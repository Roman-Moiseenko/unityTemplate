using System.Collections.Generic;
using DI;
using Game.GameRoot.View.ResourceReward;
using Game.State.Inventory.Chests;
using Game.State.Maps.Rewards;
using MVVM.UI;

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
            DIContainer container) : base(container)
        {
            TypeChest = typeChest;
            
            foreach (var reward in rewards)
            {
                var viewModel = new ResourceRewardViewModel(reward);
                RewardResources.Add(viewModel);
            }
        }
    }
}