using System;
using System.Collections.Generic;
using Game.State.Common;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace Game.State.Maps.Mobs
{
    public class MobEntity : IDisposable
    {
        private readonly MobEntityData Origin;
        
        public string ConfigId => Origin.ConfigId;
        public int UniqueId => Origin.UniqueId;
        public float SpeedMove => Origin.SpeedMove;
        public float SpeedAttack => Origin.SpeedAttack;
        public bool IsFly => Origin.IsFly;
        public int Level => Origin.Level;        
        public float Damage => Origin.Damage;   
        public float DamageSecond => Origin.DamageSecond;   
        public MobParameter? DamageSecondType => Origin.DamageSecondType;   
        public TypeDefence Defence => Origin.Defence;        
        public bool IsBoss => Origin.IsBoss;     
        public int RewardCurrency => Origin.RewardCurrency;
        public int NumberWave => Origin.NumberWave;        
        
        public ReactiveProperty<float> Health;
        public ReactiveProperty<float> Armor;       
        
        public bool IsWay => Origin.IsWay; //На главной дороге
        public float Delta;        
        
        public ReactiveProperty<Vector2> Position; //Данные не сохраняются в MobEntityData
        public ReactiveProperty<Vector2Int> Direction; //Данные не сохраняются в MobEntityData
        public ReadOnlyReactiveProperty<bool> IsDead; // = new(false);
        public readonly ReactiveProperty<Vector3> PositionTarget = new();
        public ObservableDictionary<string, MobDebuff> Debuffs = new();        
        private DisposableBag _disposables = new();

        
        public MobEntity(MobEntityData mobEntityData)
        {
            var h = mobEntityData.IsFly ? 0.55f : 0.1f;
            Origin = mobEntityData;
            
            Position = new ReactiveProperty<Vector2>(new Vector2(0,0));
            Position
                .Subscribe(newValue => PositionTarget.Value = new Vector3(newValue.x, 0 , newValue.y))
                .AddTo(ref _disposables);
            Direction = new ReactiveProperty<Vector2Int>(new Vector2Int(0, 0));
            
            Health = new ReactiveProperty<float>(mobEntityData.Health);
            Health.Subscribe(newValue => mobEntityData.Health = newValue).AddTo(ref _disposables);
            IsDead = Health.Select(x => x <= 0).ToReadOnlyReactiveProperty();
            
            Armor = new ReactiveProperty<float>(mobEntityData.Armor);
            Armor.Subscribe(newValue => mobEntityData.Armor = newValue).AddTo(ref _disposables); 
        }

        public void SetStartPosition(Vector2 position, Vector2Int direction)
        {
            Delta = Mathf.Abs(Random.insideUnitSphere.x) / 4;
            Position.Value = new Vector2(position.x + direction.x * Delta , position.y + direction.y * Delta);
            Direction.Value = direction;
        }

        public float SetDamage(float damage)
        {
            Health.Value -= damage;
            return damage;
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
            var newSpeed = SpeedMove;
            foreach (var mobDebuff in Debuffs)
            {
                if (mobDebuff.Value.Type == MobDebuffType.Speed)
                {
                    newSpeed -= SpeedMove * mobDebuff.Value.Value / 100;
                }
            }

            if (newSpeed < 0) newSpeed = 0;
            return newSpeed;
        }

        public Vector2 GetPosition()
        {
            return Position.CurrentValue;
        }

        public void Dispose()
        {
            Health?.Dispose();
            Armor?.Dispose();
            Position?.Dispose();
            Direction?.Dispose();
            IsDead?.Dispose();
            PositionTarget?.Dispose();
            _disposables.Dispose();
        }
    }
}