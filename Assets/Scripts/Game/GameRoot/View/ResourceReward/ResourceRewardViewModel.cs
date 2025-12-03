using Game.State.Inventory;
using Game.State.Maps.Rewards;

namespace Game.GameRoot.View.ResourceReward
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