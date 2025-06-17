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

        public bool IsFrame = false;
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

            /*
            foreach (var bust in entityData.Busts)
            {
                Debug.Log("BUST = " + bust);
            } */
            
            entityData.Busts.ForEach(bustOriginal =>
            {
                Busts.Add(new TowerBust
                {
                    MainAmount = bustOriginal.MainAmount,
                    MainBust = bustOriginal.MainBust,
                    SecondBust = bustOriginal.SecondBust,
                    SecondAmount = bustOriginal.SecondAmount,
                });
            });

            Busts.ObserveAdd().Subscribe(e =>
            {
                var addedEntity = e.Value;
                entityData.Busts.Add(addedEntity);
            });
            
            
        }
    }
}