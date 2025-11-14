using System.Collections.Generic;
using Game.State.Inventory.Chests;

namespace Game.Settings.Gameplay.Maps
{
    public class MapRewardSetting
    {
        public Dictionary<int, List<RewardItem>> RewardOnWave = new();
        public Dictionary<TypeChest, List<RewardItem>> RewardChest = new();
        public Dictionary<TypeChest, List<RewardItem>> RewardChanceChest = new();
    }
}