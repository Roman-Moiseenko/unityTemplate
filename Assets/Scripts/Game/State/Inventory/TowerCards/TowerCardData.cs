using System.Collections.Generic;
using Game.State.Common;
using Game.State.Inventory.Common;
using Game.State.Maps.Towers;

namespace Game.State.Inventory.TowerCards
{
    /**
     * 
     */
    public class TowerCardData : InventoryItemData
    {
        public override bool Accumulation => false;
        public override InventoryType TypeItem => InventoryType.TowerCard;
        public TypeEpic EpicLevel;
        public int Level;
        public TypeDefence Defence;
        public Dictionary<TowerParameterType, TowerParameterData> Parameters; 
        public Dictionary<TowerParameterType, TowerParameterData> BaseParameters = new();
        public Dictionary<TowerParameterType, TowerParameterData> UpdateParameters = new();
        
    }
}