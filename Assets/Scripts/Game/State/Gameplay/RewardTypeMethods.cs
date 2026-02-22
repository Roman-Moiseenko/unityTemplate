namespace Game.State.Gameplay
{
    internal static class RewardTypeMethods
    {
        public static bool IsUpgrade(this RewardType rewardType)
        {
            return rewardType is RewardType.TowerLevelUp or RewardType.HeroLevelUp or RewardType.SkillLevelUp;
        }
        

    }
}