
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

        // public List<MapData> Maps { get; set; } //Заменить на выйгранные с полученной наградой
        public List<InventoryItemData> InventoryItems { get; set; } = new(); //????
        
     //   public List<TowerCardData> TowerCards { get; set; } = new();
    //    public Dictionary<string, TowerPlanData> TowerPlans { get; set; } = new();

        public Dictionary<int, DeckCardData> DeckCards { get; set; } = new(2); //Колоды карт
        public int BattleDeck { get; set; } = 1; //Номер боевой колоды

        public int HardCurrency { get; set; }
        
      //  public bool ResumeGame { get; set; } //Привыходе false, при входе true 
        //public GameplayState GameplayState = new(); //Cостояния gameplay игры -- удалить

        public int CreateInventoryID()
        {
            return GlobalInventoryId++;
            
        }

    }    
}
