using System.Collections.Generic;

namespace Game.State.GameStates
{
    public class MapStatesData
    {
        public int LastMap = 0;
        public Dictionary<int, MapStateData> Maps  { get; set; } = new();
    }
}