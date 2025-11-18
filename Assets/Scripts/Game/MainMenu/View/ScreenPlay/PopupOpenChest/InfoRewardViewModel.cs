using System.Collections.Generic;
using Game.MainMenu.View.ScreenPlay.PopupFinishGameplay.PrefabBinders;
using Game.State.Inventory;
using Game.State.Maps.Rewards;
using MVVM.UI;

namespace Game.MainMenu.View.ScreenPlay.PopupOpenChest
{
    public class InfoRewardViewModel : DropdownViewModel
    {
        public List<ItemInfoRewardViewModel> ItemsInfoRewardViewModel = new();

        public void CreateItem(RewardEntityData rewardEntityData, bool odd)
        {
            var itemViewModel = new ItemInfoRewardViewModel();
            itemViewModel.ResourceRewardViewModel = new ResourceRewardViewModel(rewardEntityData);
            itemViewModel.Title = rewardEntityData.RewardType.GetString();
            itemViewModel.Odd = odd;
            ItemsInfoRewardViewModel.Add(itemViewModel);
        }
    }
}