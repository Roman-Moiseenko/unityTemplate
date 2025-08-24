using System.Collections.Generic;
using Game.State.Inventory.Deck;

namespace Game.State.Inventory
{
    public class InventoryRootData
    {
        public Dictionary<int, DeckCardData> DeckCards { get; set; } = new(2); //Колоды карт
        public int BattleDeck { get; set; } = 1; //Номер боевой колоды

        public List<InventoryItemData> Items = new();
        
    }
}