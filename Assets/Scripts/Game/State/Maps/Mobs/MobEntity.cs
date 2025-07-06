using R3;
using UnityEngine;

namespace Game.State.Maps.Mobs
{
    public class MobEntity
    {
        public readonly MobEntityData Origin;
        
        public string ConfigId => Origin.ConfigId;
        public int UniqueId => Origin.UniqueId;
        public ReactiveProperty<Vector3> Position; //Данные не сохраняются в MobEntityData
        public ReactiveProperty<Vector2> Direction; //Данные не сохраняются в MobEntityData
        public MobType Type => Origin.Type;
        public bool IsWay = true; //На главной дороге
        public int Speed => Origin.Speed;
        public bool IsFly => Origin.IsFly;
        public ReactiveProperty<float> Health;
        public ReactiveProperty<float> Armor;
        public ReactiveProperty<bool> IsDead = new(false);
        public float Attack => Origin.Attack;

        public int RewardCurrency => Origin.RewardCurrency;
        
        public MobEntity(MobEntityData mobEntityData)
        {
            Origin = mobEntityData;
            Position = new ReactiveProperty<Vector3>(new Vector3(0,0,0));
            //Position.Subscribe(newValue => mobEntityData.Position = newValue);
            
            Direction = new ReactiveProperty<Vector2>(new Vector2(0, 0));
            //Direction.Subscribe(newValue => mobEntityData.Direction = newValue); 
            
            Health = new ReactiveProperty<float>(mobEntityData.Health);
            Health.Subscribe(newValue => mobEntityData.Health = newValue); 
            Health.Select(x => x <= 0).Subscribe(_ => IsDead.Value = true);
            
            Armor = new ReactiveProperty<float>(mobEntityData.Armor);
            Armor.Subscribe(newValue => mobEntityData.Armor = newValue); 
        }
    }
}