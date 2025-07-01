using System.Collections.Generic;

namespace Game.State.Gameplay
{
    public class RewardsProgress
    {
    //    public RewardCardData Card1 = new();
    //    public RewardCardData Card2 = new();
     //   public RewardCardData Card3 = new();

        public Dictionary<int, RewardCardData> Cards = new(3);
    }
}