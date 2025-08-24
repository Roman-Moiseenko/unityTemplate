
using System;
using System.Collections.Generic;
//using Game.State.Entities.Buildings;
using Game.State.GameResources;
using Game.State.Inventory;
using Game.State.Inventory.Deck;
using Game.State.Inventory.TowerCards;
using Game.State.Maps;

namespace Game.State.Root
{
   // [Serializable]
    public class GameState
    {
      //  public bool HasSessionGame { get; set; }
        public int GlobalInventoryId { get; set; }
        public int CurrentMapId { get; set; } = 0;
        public int GameSpeed { get; set; } //При выходе из Gameplay сохранять
        public List<ResourceData> Resources { get; set; } = new();
        public InventoryRootData Inventory { get; set; } = new();
        
        public int HardCurrency { get; set; }
        
        public int CreateInventoryID()
        {
            return GlobalInventoryId++;
        }
    }    
}
