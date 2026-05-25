using System;
using Game.GamePlay;
using Game.State.Common;
using Game.State.Maps.Mobs;
using R3;
using UnityEngine;

namespace Game.State.Maps.Warriors
{
    public class WarriorEntity : IEntityHasHealth, IDisposable
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
        public TypeDefence Defence => Origin.Defence;
        private DisposableBag _disposables = new();

        public WarriorEntity(WarriorEntityData warriorEntityData)
        {
            Origin = warriorEntityData;
            Health = new ReactiveProperty<float>(warriorEntityData.Health);
            Health.Subscribe(v => Origin.Health = v).AddTo(ref _disposables);
            IsDead = Health.Select(v => v <= 0).ToReadOnlyReactiveProperty().AddTo(ref _disposables);
            
            Speed = new ReactiveProperty<float>(warriorEntityData.Speed);
            Speed.Subscribe(v => Origin.Speed = v).AddTo(ref _disposables);
            
            Damage = new ReactiveProperty<float>(warriorEntityData.Damage);
            Damage.Subscribe(v => Origin.Damage = v).AddTo(ref _disposables);
            
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

        public void Dispose()
        {
            Health?.Dispose();
            MaxHealth?.Dispose();
            IsDead?.Dispose();
            Speed?.Dispose();
            Damage?.Dispose();
            _disposables.Dispose();
        }
    }
}