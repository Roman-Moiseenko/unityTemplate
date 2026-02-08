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
        public ReactiveProperty<float> MaxHealth;
        public ReadOnlyReactiveProperty<bool> IsDead;
        
        public int ParentId => Origin.ParentId;
        public ReactiveProperty<float> Speed; // => Origin.Speed;
        public ReactiveProperty<float> Damage; // => Origin.Damage;
        public MobDefence Defence => Origin.Defence;

        public WarriorEntity(WarriorEntityData warriorEntityData)
        {
            Origin = warriorEntityData;
            Health = new ReactiveProperty<float>(warriorEntityData.Health);
            Health.Subscribe(v => Origin.Health = v);
            IsDead = Health.Select(v => v <= 0).ToReadOnlyReactiveProperty();
            
            Speed = new ReactiveProperty<float>(warriorEntityData.Speed);
            Speed.Subscribe(v => Origin.Speed = v);
            
            Damage = new ReactiveProperty<float>(warriorEntityData.Damage);
            Damage.Subscribe(v => Origin.Damage = v);
            
            MaxHealth = new ReactiveProperty<float>(warriorEntityData.Health);
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