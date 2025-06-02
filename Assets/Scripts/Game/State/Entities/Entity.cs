using R3;
using UnityEngine;

namespace Game.State.Entities
{
    public abstract class Entity
    {
        public EntityData Origin { get; }
        public int UniqueId => Origin.UniqueId;
        public string ConfigId => Origin.ConfigId;
        public EntityType Type => Origin.Type;
        public readonly ReactiveProperty<Vector2Int> Position;

        public Entity(EntityData entityData)
        {
            Origin = entityData;
            Position = new ReactiveProperty<Vector2Int>(entityData.Position);
            Position.Subscribe(newPosition => entityData.Position = newPosition); //При изменении позиции Position.Value меняем в данных
        }
    }
}