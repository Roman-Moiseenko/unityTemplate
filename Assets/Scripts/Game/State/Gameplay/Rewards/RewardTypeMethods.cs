namespace Game.State.Gameplay.Rewards
{
    internal static class RewardTypeMethods
    {
        public static bool IsUpgrade(this RewardType rewardType)
        {
            return rewardType is RewardType.TowerLevelUp or RewardType.HeroLevelUp or RewardType.SkillLevelUp;
        }
        
        public static bool IsTower(this RewardType rewardType)
        {
            return rewardType == RewardType.Tower;
        }
    }
}