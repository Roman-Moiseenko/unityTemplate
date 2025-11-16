namespace Game.State.Inventory
{
    public abstract class InventoryItemData
    {
        public abstract bool Accumulation { get;  }
        public abstract InventoryType TypeItem { get; }
        public string ConfigId { get; set; } //Идентификатор для поиска настроек сущности
        public long Amount { get; set; }
        public int UniqueId { get; set; }
        public string Name { get; set; }

        public T As<T>() where T : InventoryItemData
        {
            return (T)this;
        }
    }
}