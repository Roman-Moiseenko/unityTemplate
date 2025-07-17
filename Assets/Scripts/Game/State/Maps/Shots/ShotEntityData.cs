using Game.State.Maps.Mobs;
using UnityEngine;

namespace Game.State.Maps.Shots
{
    public class ShotEntityData
    {
        public string ConfigId;
        public int UniqueId;
        public int TowerEntityId;
        public int MobEntityId;
        public Vector3 StartPosition;
        public Vector3 FinishPosition;
        public Vector3 Position;
        public float Speed;
        public float Damage;
        public bool Single;
        public bool NotPrefab;

        public MobDebuff Debuff;
    }
}