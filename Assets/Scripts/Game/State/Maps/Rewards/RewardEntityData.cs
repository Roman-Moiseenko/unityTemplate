using Game.State.Inventory;
using Game.State.Inventory.Common;

namespace Game.State.Maps.Rewards
{
    public class RewardEntityData
    {
        public string ConfigId;
        public InventoryType RewardType;
        public long Amount = 0;
    }
}