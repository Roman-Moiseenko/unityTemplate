using R3;

namespace Game.State.Inventory
{
    public abstract class Inventory
    {
        public InventoryData Origin { get; }
        public InventoryType TypeItem => Origin.TypeItem;
        public string ConfigId => Origin.ConfigId;
        public int UniqueId => Origin.UniqueId;
        public readonly ReactiveProperty<int> Amount;
        public readonly ReactiveProperty<int> Level;
        

        public Inventory(InventoryData data)
        {
            Origin = data;
            Amount = new ReactiveProperty<int>(data.Amount);
            Amount.Subscribe(newAmount => data.Amount = newAmount); 
            
            Level = new ReactiveProperty<int>(data.Level);
            Level.Subscribe(newAmount => data.Level = newAmount);
        }

        public T As<T>() where T : Inventory
        {
            return (T)this;
        }
    }
}