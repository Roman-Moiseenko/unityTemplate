using R3;

namespace Game.State.Mergeable.Buildings
{
    public class BuildingEntity : MergeableEntity
    {
        public readonly ReactiveProperty<double> LastClickedTimeMS;
        public readonly ReactiveProperty<bool> IsAutoCollectionEnabled;


        public BuildingEntity(BuildingEntityData data) : base(data)
        {
            LastClickedTimeMS = new ReactiveProperty<double>(data.LastClickedTimeMS);
            LastClickedTimeMS.Subscribe(newValue => data.LastClickedTimeMS = newValue);

            IsAutoCollectionEnabled = new ReactiveProperty<bool>(data.IsAutoCollectionEnabled);
            IsAutoCollectionEnabled.Subscribe(newValue => data.IsAutoCollectionEnabled = newValue);

        }
    }
}