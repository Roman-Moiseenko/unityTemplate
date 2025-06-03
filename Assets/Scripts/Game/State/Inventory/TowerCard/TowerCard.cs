using Game.State.Maps.Towers;

namespace Game.State.Inventory.TowerCard
{
    public class TowerCard : Inventory
    {
        //public TowerType TowerType { get; }
        public int EpicLevel { get; }

        public TowerCard(TowerCardData data) : base(data)
        {
            EpicLevel = data.EpicLevel;
            //     TowerType = data.TowerType;
        }
    }
}