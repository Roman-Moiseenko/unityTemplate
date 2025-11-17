using System.Collections.Generic;

namespace Game.State.GameStates
{
    public class ResultRewards
    {
        public long SoftCurrency = 0;
        public Dictionary<string, long> TowerCards = new();
        public Dictionary<string, long> TowerPlans = new();
    }
}