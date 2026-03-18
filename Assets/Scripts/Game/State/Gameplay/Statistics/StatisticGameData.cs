using System.Collections.Generic;

namespace Game.State.Gameplay.Statistics
{
    public class StatisticGameData
    {
        public int CountKills { get; set; }
        public int CountTowers { get; set; }
        public int CountRoads { get; set; }
        
        public float AllDamage { get; set; }

        public List<PairStringFloat> Damages = new();
    }
}
