using Game.State.Maps.Towers;

namespace Game.State.Inventory.TowerCard
{
    public class TowerCard : Inventory
    {
        public TowerType TowerType { get; }

        public TowerCard(TowerCardData data) : base(data)
        {
            TowerType = data.TowerType;
        }
    }
}