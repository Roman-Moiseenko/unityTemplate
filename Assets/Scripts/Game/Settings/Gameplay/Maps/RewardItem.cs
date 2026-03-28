using Game.State.Inventory;
using Game.State.Inventory.Common;

namespace Game.Settings.Gameplay.Maps
{
    
    //TODO Заменить на RewardEntityData и на Web-сервере
    public class RewardItem
    {
        public string ConfigId = "";
        public InventoryType Type = InventoryType.SoftCurrency;
        public int Amount = 0;
    }
}