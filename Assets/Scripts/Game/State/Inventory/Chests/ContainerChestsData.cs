using System.Collections.Generic;

namespace Game.State.Inventory.Chests
{
    public class ContainerChestsData
    {
        public Dictionary<int, ChestEntityData> Chests = new();
        
        public long StartOpening;
        public int CellOpening;
        
    }
}