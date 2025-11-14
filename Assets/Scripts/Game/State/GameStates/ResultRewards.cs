using System.Collections.Generic;

namespace Game.State.GameStates
{
    public class ResultRewards
    {
        public int SoftCurrency = 0;
        public Dictionary<string, int> TowerCards = new();
        public Dictionary<string, int> TowerPlans = new();
    }
}