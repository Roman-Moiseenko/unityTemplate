using Game.State.Entities;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Game.State.Maps.Towers
{
    public class TowerEntity : Entity
    {
        public readonly TowerTypeEnemy TypeEnemy;
        
        public readonly ReactiveProperty<double> Damage;
        public readonly ReactiveProperty<double> Speed;
        
        public ObservableDictionary<TowerParameterType, TowerParameterData> Parameters;

        
        public TowerEntity(TowerEntityData entityData) : base(entityData)
        {
        //    Origin = entityData;
            TypeEnemy = entityData.TypeEnemy;
            
            Damage = new ReactiveProperty<double>(entityData.Damage);
            Damage.Subscribe(newValue => entityData.Damage = newValue);
            Speed = new ReactiveProperty<double>(entityData.Speed);
            Speed.Subscribe(newValue => entityData.Speed = newValue);

            Parameters = new ObservableDictionary<TowerParameterType, TowerParameterData>();
            
        }

        public bool PositionNear(Vector2Int position)
        {
            var x = Position.CurrentValue.x;
            var y = Position.CurrentValue.y;
            if (position.x == x && position.y == y - 1) return true;
            if (position.x == x && position.y == y + 1) return true;
            if (position.x == x - 1 && position.y == y) return true;
            if (position.x == x + 1 && position.y == y) return true;
            return false;
        }
    }
}