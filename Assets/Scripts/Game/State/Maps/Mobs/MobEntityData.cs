using Game.Settings.Gameplay.Enemies;
using UnityEngine;

namespace Game.State.Maps.Mobs
{
    public class MobEntityData
    {
        public string ConfigId;
        public int UniqueId;
        public bool IsBoss;
        public int Level;
        public MobDefence Defence;
        public bool IsFly;
        public int NumberWave;       
        
        public float SpeedMove;
        public float SpeedAttack;
        public float Health;
        public float Armor;
        public float Damage;

        public float DamageSecond;
        public MobParameter? DamageSecondType;

        public int RewardCurrency;
        //public MobState State = MobState.Moving;


    }
}