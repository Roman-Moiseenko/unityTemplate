using System.Collections.Generic;
using Game.State.Maps.Mobs;
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
        public TypeEpicCard EpicLevel;
        public int Level;
        public MobDefence Defence;
        public Dictionary<TowerParameterType, TowerParameterData> Parameters; 
        public Dictionary<TowerParameterType, TowerParameterData> BaseParameters = new();
        public Dictionary<TowerParameterType, TowerParameterData> UpdateParameters = new();
        
        
    }
}