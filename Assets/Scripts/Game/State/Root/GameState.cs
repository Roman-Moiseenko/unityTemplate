
using System;
using System.Collections.Generic;
using Game.State.Entities.Buildings;
using Game.State.GameResources;
using Game.State.Maps;

namespace Game.State.Root
{
    [Serializable]
    public class GameState
    {
        public int GlobalEntityId;
        public int CurrentMapId;
        
        public List<MapState> Maps;
        public List<ResourceData> Resources;
        
        public int CreateEntityID()
        {
            return GlobalEntityId++;
        }

    }    
}
