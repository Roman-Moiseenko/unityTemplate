using Game.GamePlay;
using R3;
using UnityEngine;

namespace Game.State.Maps.Warriors
{
    public class WarriorEntity : IEntityHasHealth
    {
        public WarriorEntityData Origin { get; }
        public int UniqueId => Origin.UniqueId;
        public string ConfigId => Origin.ConfigId;
        public bool IsFly => Origin.IsFly;
        public Vector2Int StartPosition => Origin.StartPosition;
        public Vector2Int PlacementPosition => Origin.PlacementPosition;
        //TODO Удалить Position - динамическая величина, по ходу игры, не сохраняется
        public ReactiveProperty<Vector3> Position; //Позиция на карте для движения
        public ReactiveProperty<float> Health;
        public ReadOnlyReactiveProperty<bool> IsDead;

        public ReactiveProperty<int> TargetId;
        public int ParentId => Origin.ParentId;
        public ReactiveProperty<bool> IsBusy = new(false);
        public float Speed => Origin.Speed;
        public float Damage => Origin.Damage;
        
        public WarriorEntity(WarriorEntityData warriorEntityData)
        {
            Origin = warriorEntityData;
            Position = new ReactiveProperty<Vector3>(
                new Vector3(
                    warriorEntityData.StartPosition.x,
                    warriorEntityData.IsFly ? 1 : 0,
                    warriorEntityData.StartPosition.y
                )
                );
            //Position.Subscribe(v => Origin.Position = v);
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