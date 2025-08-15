using System.Collections.Generic;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using UnityEngine;
using UnityEngine.Rendering;

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
        public float BaseSpeed => Origin.Speed;
        public bool IsFly => Origin.IsFly;
        public int Level => Origin.Level;
        public ReactiveProperty<float> Health;
        public ReactiveProperty<float> Armor;
        public ReactiveProperty<bool> IsDead = new(false);
        public float Attack => Origin.Attack;
        public float Delta;
        public ReactiveProperty<MobState> State;

        public ObservableDictionary<string, MobDebuff> Debuffs = new();

        public readonly ReactiveProperty<Vector3> PositionTarget = new();

        public int RewardCurrency => Origin.RewardCurrency;
        
        public MobEntity(MobEntityData mobEntityData)
        {
            Origin = mobEntityData;
            State = new ReactiveProperty<MobState>(mobEntityData.State);
            State.Subscribe(s => mobEntityData.State = s);
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

        public float SetDamage(float damage)
        {
            var damageReceived = damage - Armor.Value;
            Health.Value -= damageReceived;
            return damageReceived;
        }
        
        public void SetDebuff(string configId, MobDebuff debuff)
        {
            Debuffs.TryAdd(configId, debuff);
        }

        public void RemoveDebuff(string configId)
        {
            Debuffs.Remove(configId);
        }

        public float Speed()
        {
            var newSpeed = BaseSpeed;
            foreach (var mobDebuff in Debuffs)
            {
                if (mobDebuff.Value.Type == MobDebuffType.Speed)
                {
                    newSpeed -= BaseSpeed * mobDebuff.Value.Value / 100;
                }
            }

            if (newSpeed < 0) newSpeed = 0;
            return newSpeed;
        }

        public Vector2 GetPosition()
        {
            return Position.CurrentValue;
        }
    }
}