using R3;
using UnityEngine;

namespace Game.State.Maps.Warriors
{
    public class WarriorEntity
    {
        public WarriorEntityData Origin { get; }
        public int UniqueId => Origin.UniqueId;
        public string ConfigId => Origin.ConfigId;
        public bool IsFly => Origin.IsFly;
        public Vector3 StartPosition => Origin.StartPosition;
        public ReactiveProperty<Vector3> Position;
        public ReactiveProperty<float> Health;
        public ReadOnlyReactiveProperty<bool> IsDead;

        public ReactiveProperty<int> TargetId;
        public int ParentId => Origin.ParentId;

        public WarriorEntity(WarriorEntityData warriorEntityData)
        {
            Origin = warriorEntityData;
            Position = new ReactiveProperty<Vector3>(warriorEntityData.Position);
            Position.Subscribe(v => Origin.Position = v);
            Health = new ReactiveProperty<float>(warriorEntityData.Health);
            Health.Subscribe(v => Origin.Health = v);
            IsDead = Health.Select(v => v <= 0).ToReadOnlyReactiveProperty();
            
        }
    }
}