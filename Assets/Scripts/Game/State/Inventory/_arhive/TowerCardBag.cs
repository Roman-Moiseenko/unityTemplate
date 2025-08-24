using UnityEngine;

namespace Game.State.Inventory.TowerCards
{
    public class TowerCardBag : InventoryBag<TowerCard, TowerCardData>
    {
        public override bool Accumulation => false;

        public TowerCardBag(InventoryBagData<TowerCardData> bagData): base(bagData)
        {
        }
    }
}