using System.Collections.Generic;
using Game.State.Maps.Towers;

namespace Game.State.Inventory.TowerCards
{
    /**
     * 
     */
    public class TowerCardData : InventoryItemData
    {
        public override bool Accumulation => false;
        public TypeEpicCard EpicLevel;
        public int Level;
        public Dictionary<TowerParameterType, TowerParameterData> Parameters; 
        public Dictionary<TowerParameterType, TowerParameterData> BaseParameters = new();
        public Dictionary<TowerParameterType, TowerParameterData> UpdateParameters = new();
        
        public TowerCardData()
        {
            TypeItem = InventoryType.TowerCard;
        }

        
    }
}