
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
        public int CurrentMapId { get; set; }
        public int GameSpeed { get; set; } //При выходе из Gameplay сохранять

        public List<ResourceData> Resources { get; set; }
       // public List<MapData> Maps { get; set; } //Заменить на выйгранные с полученной наградой
        public List<InventoryItemData> Inventory { get; set; } //????
        
        public List<TowerCardData> TowerCards { get; set; }
        public Dictionary<string, TowerPlanData> TowerPlans { get; set; }
        
        public Dictionary<int, DeckCardData> DeckCards { get; set; } //Колоды карт
        public int BattleDeck { get; set; } //Номер боевой колоды

      //  public bool ResumeGame { get; set; } //Привыходе false, при входе true 
        //public GameplayState GameplayState = new(); //Cостояния gameplay игры -- удалить
        
        public int CreateInventoryID()
        {
            return GlobalInventoryId++;
            
        }

    }    
}
