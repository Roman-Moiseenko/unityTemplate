using System.Linq;
using Game.State.Inventory.Deck;
using Game.State.Inventory.TowerCards;
using Game.State.Maps.Rewards;
using ObservableCollections;
using R3;
using UnityEngine;
using DeckCard = Game.State.Inventory.Deck.DeckCard;


namespace Game.State.Inventory
{
    public class InventoryRoot
    {
        public InventoryRootData Origin;
        public ObservableDictionary<int, DeckCard> DeckCards { get; set; } //Колоды карт
        public ReactiveProperty<int> BattleDeck;
        public IObservableCollection<InventoryItem> Items => _items;
        
        private readonly ObservableList<InventoryItem> _items = new();

        public Subject<Unit> UpdateData = new();

        public InventoryRoot(InventoryRootData rootData)
        {
            Origin = rootData;

            foreach (var itemData in rootData.Items)
            {
                var itemEntity = InventoryFactory.CreateInventory(itemData);
                itemEntity.Amount
                    .Where(x => x == 0)
                    .Subscribe(_ =>
                    {
                        _items.Remove(itemEntity);
                        UpdateData.OnNext(Unit.Default);
                    });
                
                _items.Add(itemEntity);
            }

            _items.ObserveAdd().Subscribe(e =>
            {
                var itemEntity = e.Value;
                itemEntity.Amount
                    .Where(x => x == 0)
                    .Subscribe(_ =>
                    {
                        _items.Remove(itemEntity);
                        UpdateData.OnNext(Unit.Default);
                    });
                Origin.Items.Add(e.Value.Origin);
            });
            _items.ObserveRemove().Subscribe(e =>
            {
                Origin.Items.Remove(e.Value.Origin);
                UpdateData.OnNext(Unit.Default);
            });

            BattleDeck = new ReactiveProperty<int>(rootData.BattleDeck);
            BattleDeck.Subscribe(newValue =>
            {
                rootData.BattleDeck = newValue;
                UpdateData.OnNext(Unit.Default);
            });
            
            DeckCards = new ObservableDictionary<int, DeckCard>();

            foreach (var keyValue in rootData.DeckCards)
            {
                DeckCards.Add(keyValue.Key, new DeckCard(keyValue.Value));
            }

            DeckCards.ObserveAdd().Subscribe(e =>
            {
                rootData.DeckCards.Add(e.Value.Key, e.Value.Value.Origin);
                UpdateData.OnNext(Unit.Default);
            });
            DeckCards.ObserveRemove().Subscribe(e =>
            {
                rootData.DeckCards.Remove(e.Value.Key);
                UpdateData.OnNext(Unit.Default);
            });
        }

        public void AddItem(InventoryItemData item)
        {
            if (item.TypeItem.IsAccumulation())
            {
                foreach (var inventoryItem in _items)
                {
                    if (inventoryItem.TypeItem == item.TypeItem && inventoryItem.ConfigId == item.ConfigId)
                    {
                        inventoryItem.Amount.Value += item.Amount;
                        return;
                    }
                }
            }

            var entity = InventoryFactory.CreateInventory(item);
            _items.Add(entity);
        }

        public DeckCard GetCurrentDeckCard()
        {
            return DeckCards[BattleDeck.CurrentValue];
        }

        public T Get<T>(int uniqueId) where T : InventoryItem
        {
            var item = _items.FirstOrDefault(item => item.UniqueId == uniqueId);
            return item?.As<T>();
        }

        public T GetByConfigAndType<T>(InventoryType type, string configId) where T : InventoryItem
        {
            var item = _items.FirstOrDefault(i => i.ConfigId == configId 
                                                  && i.TypeItem == InventoryType.TowerPlan);
            return item?.As<T>();
        }

        public void RemoveItem(InventoryItem item)
        {
            _items.Remove(item);
        }

        public void RemoveItem(int uniqueId)
        {
            var item = _items.FirstOrDefault(item => item.UniqueId == uniqueId);
            RemoveItem(item);
        }
    }
}