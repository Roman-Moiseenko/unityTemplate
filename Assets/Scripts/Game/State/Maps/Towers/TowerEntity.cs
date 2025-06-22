using Game.State.Entities;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.State.Maps.Towers
{
    public class TowerEntity : Entity
    {
        public readonly TowerTypeDamage TypeDamage;
        public readonly TowerTypeEnemy TypeEnemy;
        
        public readonly ReactiveProperty<double> Damage;
        public readonly ReactiveProperty<double> Speed;

        public ObservableList<TowerBust> Busts { get; } = new();
        
        public TowerEntity(TowerEntityData entityData) : base(entityData)
        {
        //    Origin = entityData;
            TypeEnemy = entityData.TypeEnemy;
            TypeDamage = entityData.TypeDamage;
            
            Damage = new ReactiveProperty<double>(entityData.Damage);
            Damage.Subscribe(newValue => entityData.Damage = newValue);
            Speed = new ReactiveProperty<double>(entityData.Speed);
            Speed.Subscribe(newValue => entityData.Speed = newValue);
            
            
        }
    }
}