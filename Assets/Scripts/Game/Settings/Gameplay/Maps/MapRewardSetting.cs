using System.Collections.Generic;
using Game.State.Inventory.Chests;
using Game.State.Maps.Rewards;

namespace Game.Settings.Gameplay.Maps
{
    public class MapRewardSetting
    {
        public Dictionary<int, List<RewardEntityData>> RewardOnWave = new();
        public Dictionary<TypeChest, List<RewardEntityData>> RewardChest = new();
        public Dictionary<TypeChest, List<RewardEntityData>> RewardChanceChest = new();
    }
}