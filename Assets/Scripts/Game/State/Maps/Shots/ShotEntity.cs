using Game.State.Maps.Mobs;
using UnityEngine;
using R3;
using Unity.VisualScripting;

namespace Game.State.Maps.Shots
{
    public class ShotEntity
    {
        public ShotEntityData Origin;
        public string ConfigId => Origin.ConfigId;
        public int TowerEntityId => Origin.TowerEntityId;
        public int MobEntityId => Origin.MobEntityId;
        public int UniqueId => Origin.UniqueId;
        public Vector3 StartPosition => Origin.StartPosition;
        public float Speed => Origin.Speed;
        public bool Single => Origin.Single;
        
        public ReactiveProperty<Vector3> Position;
        public ReactiveProperty<Vector3> FinishPosition;
        public float Damage => Origin.Damage;
        public bool NotPrefab => Origin.NotPrefab;
        public MobDebuff Debuff => Origin.Debuff;

        public ShotEntity(ShotEntityData shotEntityData, ReactiveProperty<Vector3> position)
        {
            Origin = shotEntityData;
            Position = new ReactiveProperty<Vector3>(shotEntityData.Position);
            Position.Subscribe(newValue => shotEntityData.Position = newValue);
            
            FinishPosition = position; //Конечная позиция снаряда меняется при движении моба
            FinishPosition.Subscribe(newValue => shotEntityData.FinishPosition = newValue);
            
        }
        
    }
}