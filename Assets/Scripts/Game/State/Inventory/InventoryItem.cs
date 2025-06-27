using R3;

namespace Game.State.Inventory
{
    public abstract class InventoryItem
    {
        public InventoryItemData Origin { get; }
        public InventoryType TypeItem => Origin.TypeItem;
        public string ConfigId => Origin.ConfigId;
        public int UniqueId => Origin.UniqueId;
        public readonly ReactiveProperty<int> Amount;

        
        public InventoryItem(InventoryItemData data)
        {
            Origin = data;
            Amount = new ReactiveProperty<int>(data.Amount);
            Amount.Subscribe(newAmount => data.Amount = newAmount); 
  
        }

        public T As<T>() where T : InventoryItem
        {
            return (T)this;
        }
    }
}