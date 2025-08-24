namespace Game.State.Inventory
{
    public abstract class InventoryItemData
    {
        public abstract bool Accumulation { get;  }
        public  InventoryType TypeItem { get; set; }
        public string ConfigId { get; set; } //Идентификатор для поиска настроек сущности
        public int Amount { get; set; }
        public int UniqueId { get; set; }
    }
}