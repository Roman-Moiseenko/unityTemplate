using System.Collections.Generic;
using Game.State.Inventory.Chests;

namespace Game.State.GameStates
{
    public class MapStateData
    {
        public int MapId;
        public bool Finished;
        public List<ResultMap> Results;
        public int RewardOnWave = 0;
        public TypeChest RewardChest;
        
        //TODO Список максимально полученных наград за волны и сундуки
        //public 
    }
}