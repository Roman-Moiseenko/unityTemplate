using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Game.State.Inventory
{
    public abstract class InventoryBagData<T> where T : InventoryItemData
    {
        public List<T> Items = new();
    }
}