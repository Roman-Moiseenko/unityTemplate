using Game.State.Maps.Mobs;
using UnityEngine;

namespace Game.State.Maps.Shots
{
    public class ShotData
    {
        public string ConfigId; //Для дебафа, чтоб не складывались от одной башни
        public int MobEntityId;
        public MobEntity MobEntity;
        public Vector3 Position;
        public float Damage;
        public bool Single;
        public MobDebuff Debuff;
        public DamageType DamageType;
        public bool IsFly;
    }
}