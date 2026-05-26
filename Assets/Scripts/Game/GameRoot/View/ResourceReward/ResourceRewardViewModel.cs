using Game.State.Inventory;
using Game.State.Inventory.Common;
using Game.State.Maps.Rewards;

namespace Game.GameRoot.View.ResourceReward
{
    public class ResourceRewardViewModel
    {
        public readonly long Amount;
        public readonly InventoryType InventoryType;
        public readonly string ConfigId;

        public ResourceRewardViewModel(RewardEntityData rewardEntityData)
        {
            InventoryType = rewardEntityData.RewardType;
            ConfigId = rewardEntityData.ConfigId == "" ? "UnKnow" : rewardEntityData.ConfigId;
            Amount = rewardEntityData.Amount;
        }
    }
}