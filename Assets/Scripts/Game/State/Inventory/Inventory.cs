using R3;

namespace Game.State.Inventory
{
    public abstract class Inventory
    {
        public InventoryData Origin { get; }
        public InventoryType TypeItem => Origin.TypeItem;
        public string ConfigId => Origin.ConfigId;
        public readonly ReactiveProperty<int> Amount;

        public Inventory(InventoryData data)
        {
            Origin = data;
            Amount = new ReactiveProperty<int>(data.Amount);
            Amount.Subscribe(newAmount => data.Amount = newAmount); 
        }
    }
}