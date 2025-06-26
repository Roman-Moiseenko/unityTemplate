using System.Collections.Generic;
using Game.State.Maps.Towers;

namespace Game.State.Inventory.TowerCard
{
    public class TowerCardData : InventoryData
    {
        public int EpicLevel;
        
        public Dictionary<TowerParameterType, TowerParameterData> Parameters;
    }
}