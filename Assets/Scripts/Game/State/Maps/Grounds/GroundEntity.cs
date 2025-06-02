using Game.State.Entities;
using R3;

namespace Game.State.Maps.Grounds
{
    public class GroundEntity : Entity
    {
 //       public GroundType GroundType;
        public readonly ReactiveProperty<bool> Enabled;
        public GroundEntity(GroundEntityData entityData) : base(entityData)
        {
         //   GroundType = entityData.GroundType;
            Enabled = new ReactiveProperty<bool>(entityData.Enabled);
            Enabled.Subscribe(newValue => entityData.Enabled = newValue);
        }
    }
}