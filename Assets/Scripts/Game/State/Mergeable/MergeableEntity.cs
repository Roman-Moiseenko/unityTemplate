using Game.State.Entities;
using R3;

namespace Game.State.Mergeable
{
    public abstract class MergeableEntity : Entity
    {
        protected MergeableEntity(MergeableEntityData data) : base(data)
        {
        }

    }
}