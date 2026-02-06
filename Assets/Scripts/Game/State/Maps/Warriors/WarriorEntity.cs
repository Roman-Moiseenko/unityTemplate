using Game.GamePlay;
using Game.State.Maps.Mobs;
using R3;
using UnityEngine;

namespace Game.State.Maps.Warriors
{
    public class WarriorEntity : IEntityHasHealth
    {
        public int Index;
        public WarriorEntityData Origin { get; }
        public int UniqueId => Origin.UniqueId;
        public string ConfigId => Origin.ConfigId;
        public bool IsFly => Origin.IsFly;
        
        public ReactiveProperty<float> Health;
        public ReadOnlyReactiveProperty<bool> IsDead;
        
        public int ParentId => Origin.ParentId;
        public float Speed => Origin.Speed;
        public float Damage => Origin.Damage;
        public MobDefence Defence => Origin.Defence;

        public WarriorEntity(WarriorEntityData warriorEntityData)
        {
            Origin = warriorEntityData;
            Health = new ReactiveProperty<float>(warriorEntityData.Health);
            Health.Subscribe(v => Origin.Health = v);
            IsDead = Health.Select(v => v <= 0).ToReadOnlyReactiveProperty();
        }

        public void DamageReceived(float damage)
        {
            Health.Value -= damage;
        }

        public bool IsDeadEntity()
        {
            return IsDead.CurrentValue;
        }
    }
}