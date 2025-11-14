using System;

namespace Game.State.GameStates
{
    public class ResultMap
    {
        public DateTime DatePays;
        public int LastWave = 0;
        public bool Finished = false;
        public ResultRewards Reward;
    }
}