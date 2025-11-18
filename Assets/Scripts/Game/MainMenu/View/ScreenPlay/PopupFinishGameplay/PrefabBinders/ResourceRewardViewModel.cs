using Game.State.Inventory;
using Game.State.Maps.Rewards;
using UnityEngine;

namespace Game.MainMenu.View.ScreenPlay.PopupFinishGameplay.PrefabBinders
{
    public class ResourceRewardViewModel
    {
        public long Amount;
        public InventoryType InventoryType;
        public string ConfigId;

        public ResourceRewardViewModel(RewardEntityData rewardEntityData)
        {
            InventoryType = rewardEntityData.RewardType;
            ConfigId = rewardEntityData.ConfigId == "" ? "UnKnow" : rewardEntityData.ConfigId;
            Amount = rewardEntityData.Amount;
        }
    }
}