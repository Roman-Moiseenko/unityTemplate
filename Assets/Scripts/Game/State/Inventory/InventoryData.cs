namespace Game.State.Inventory
{
    public class InventoryData
    {
        public InventoryType TypeItem { get; set; }
        public string ConfigId { get; set; } //Идентификатор для поиска настроек сущности
        public int Level { get; set; }
        public int Amount { get; set; }
        
        public int UniqueId { get; set; }
    }
}