
using System;
using System.Collections.Generic;
//using Game.State.Entities.Buildings;
using Game.State.GameResources;
using Game.State.Inventory;
using Game.State.Maps;

namespace Game.State.Root
{
   // [Serializable]
    public class GameState
    {
        public int GlobalEntityId { get; set; }
        public int CurrentMapId { get; set; }
        
        public List<MapData> Maps { get; set; }
        public List<InventoryData> Inventory { get; set; }
        public List<ResourceData> Resources { get; set; }
        
        public int CreateEntityID()
        {
            return GlobalEntityId++;
        }

    }    
}
