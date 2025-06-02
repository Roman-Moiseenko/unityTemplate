namespace Game.State.Inventory
{
    public class InventoryData
    {
        public InventoryType TypeItem { get; set; }
        public string ConfigId { get; set; } //Идентификатор для поиска настроек сущности
        public int Amount { get; set; }
    }
}