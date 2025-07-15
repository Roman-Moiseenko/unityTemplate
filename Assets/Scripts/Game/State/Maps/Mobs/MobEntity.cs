using R3;
using UnityEngine;

namespace Game.State.Maps.Mobs
{
    public class MobEntity
    {
        public readonly MobEntityData Origin;
        
        public string ConfigId => Origin.ConfigId;
        public int UniqueId => Origin.UniqueId;
        public ReactiveProperty<Vector2> Position; //Данные не сохраняются в MobEntityData
        public ReactiveProperty<Vector2Int> Direction; //Данные не сохраняются в MobEntityData
        public MobType Type => Origin.Type;
        public bool IsWay = true; //На главной дороге
        public int Speed => Origin.Speed;
        public bool IsFly => Origin.IsFly;
        public ReactiveProperty<float> Health;
        public ReactiveProperty<float> Armor;
        public ReactiveProperty<bool> IsDead = new(false);
        public float Attack => Origin.Attack;
        public float Delta;

        public readonly ReactiveProperty<Vector3> PositionTarget = new();

        public int RewardCurrency => Origin.RewardCurrency;
        
        public MobEntity(MobEntityData mobEntityData)
        {
            Origin = mobEntityData;
            Position = new ReactiveProperty<Vector2>(new Vector2(0,0));
            Position.Subscribe(newValue =>
            {
                //TODO Менять высоту в зависимости от IsFly ??
                var vector = new Vector3(newValue.x, 0.5f , newValue.y);
                PositionTarget.Value = vector;
            });
            
            Direction = new ReactiveProperty<Vector2Int>(new Vector2Int(0, 0));
            //Direction.Subscribe(newValue => mobEntityData.Direction = newValue); 
            
            Health = new ReactiveProperty<float>(mobEntityData.Health);
            Health.Subscribe(newValue => mobEntityData.Health = newValue); 
            Health.Select(x => x <= 0).Subscribe(h =>
            {
                IsDead.Value = h;
            });
            
            Armor = new ReactiveProperty<float>(mobEntityData.Armor);
            Armor.Subscribe(newValue => mobEntityData.Armor = newValue); 
        }

        public void SetStartPosition(Vector2 position, Vector2Int direction)
        {
            Delta = Mathf.Abs(Random.insideUnitSphere.x) / 4;
            Position.Value = new Vector2(position.x + direction.x * Delta , position.y + direction.y * Delta);
            Direction.Value = direction;
        }

        public void SetDamage(float damage)
        {
            if (damage > Armor.Value)
            {
                Health.Value -= damage - Armor.Value;
            }
            else
            {
                Armor.Value -= damage;
            }
        }
    }
}