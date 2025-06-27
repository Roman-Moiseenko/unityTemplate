using System.Collections.Generic;
using Game.State.Maps.Towers;

namespace Game.State.Inventory.TowerCards
{
    /**
     * 
     */
    public class TowerCardData : InventoryItemData
    {
        public TypeEpicCard EpicLevel;
        public int Level;
        public Dictionary<TowerParameterType, TowerParameterData> Parameters;
    }
}