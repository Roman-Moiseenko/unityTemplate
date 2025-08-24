using Game.State.Inventory.Deck;
using ObservableCollections;
using R3;


namespace Game.State.Inventory
{
    public class InventoryRoot
    {
        public InventoryRootData Origin;
        public ObservableDictionary<int, DeckCard> DeckCards { get; set; } //Колоды карт
        public ReactiveProperty<int> BattleDeck;
        public ObservableList<InventoryItem> Items = new();
        
        public InventoryRoot(InventoryRootData rootData)
        {
            Origin = rootData;

            foreach (var itemData in rootData.Items)
            {
                Items.Add(InventoryFactory.CreateInventory(itemData));
            }

            Items.ObserveAdd().Subscribe(e => { Origin.Items.Add(e.Value.Origin); });
            Items.ObserveRemove().Subscribe(e => { Origin.Items.Remove(e.Value.Origin); });
            
            BattleDeck = new ReactiveProperty<int>(rootData.BattleDeck);
            BattleDeck.Subscribe(newValue => rootData.BattleDeck = newValue);
            DeckCards = new ObservableDictionary<int, DeckCard>();

            foreach (var keyValue in rootData.DeckCards)
            {
                DeckCards.Add(keyValue.Key, new DeckCard(keyValue.Value));
            }

            DeckCards.ObserveAdd().Subscribe(e => { rootData.DeckCards.Add(e.Value.Key, e.Value.Value.Origin); });
            DeckCards.ObserveRemove().Subscribe(e => { rootData.DeckCards.Remove(e.Value.Key); });
        }

        public void AddItem(InventoryItemData item)
        {
            if (item.Accumulation)
            {
                foreach (var inventoryItem in Items)
                {
                    if (inventoryItem.TypeItem == item.TypeItem && inventoryItem.ConfigId == item.ConfigId)
                    {
                        inventoryItem.Amount.Value += item.Amount;
                        return;
                    }
                }
            }

            var entity = InventoryFactory.CreateInventory(item);
            Items.Add(entity);
        }

        public DeckCard GetCurrentDeckCard()
        {
            return DeckCards[BattleDeck.CurrentValue];
        }
    }
}