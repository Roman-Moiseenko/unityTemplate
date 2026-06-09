using System.Collections.Generic;
using Game.State.Common;
using Game.State.Inventory.Common;
using Game.State.Maps.Towers;
using Game.State.Parameters;

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
        public Dictionary<ParameterType, ParameterData> Parameters; 
        public Dictionary<ParameterType, ParameterData> BaseParameters = new();
        public Dictionary<ParameterType, ParameterData> UpdateParameters = new();
        
    }
}