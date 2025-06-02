namespace Game.State.Mergeable.Buildings
{
    public class BuildingEntityData : MergeableEntityData
    {
        public double LastClickedTimeMS { get; set; } //Время последнего клика
        public bool IsAutoCollectionEnabled { get; set; } //Автосбор
    }
}