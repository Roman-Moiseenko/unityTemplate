using Game.State.Entities;
using R3;

namespace Game.State.Mergeable
{
    public abstract class MergeableEntity : Entity
    {
        public readonly ReactiveProperty<int> Level;
        protected MergeableEntity(MergeableEntityData data) : base(data)
        {
            Level = new ReactiveProperty<int>(data.Level);
            Level.Subscribe(newLevel => data.Level = newLevel);
        }

    }
}