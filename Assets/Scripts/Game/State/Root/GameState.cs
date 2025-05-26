
using System;
using System.Collections.Generic;
using Game.State.Entities.Buildings;
using Game.State.Maps;

namespace Game.State.Root
{
    [Serializable]
    public class GameState
    {
        public int GlobalEntityId;
        public List<MapState> Maps;
        public int CurrentMapId;
        
        public int CreateEntityID()
        {
            return GlobalEntityId++;
        }

    }    
}
