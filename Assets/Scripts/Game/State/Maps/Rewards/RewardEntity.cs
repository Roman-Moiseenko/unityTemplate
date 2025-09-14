using Game.State.Inventory;
using UnityEngine;

namespace Game.State.Maps.Rewards
{
    public class RewardEntity
    {
        public RewardEntityData Origin;
        public string ConfigId => Origin.ConfigId;
        public InventoryType RewardType => Origin.RewardType;
        public Vector2 Position;

        public RewardEntity(RewardEntityData entityData)
        {
            Origin = entityData;
            Position = Vector2.zero;
        }
    }
}