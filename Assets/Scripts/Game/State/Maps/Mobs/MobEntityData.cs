﻿using Game.Settings.Gameplay.Enemies;
using UnityEngine;

namespace Game.State.Maps.Mobs
{
    public class MobEntityData
    {
        public string ConfigId;
        public int UniqueId;
       // public Vector3 Position;
        //public Vector2 Direction;
        public MobType Type;
        public int Level;
        
        
        public bool IsFly;
        
        public float Speed = 1f;
        public float Health;
        public float Armor;
        public float Attack;

        public int RewardCurrency;
        public MobState State = MobState.Moving;

        //TODO Тип урона

        /*    public MobEntityData(MobSettings settings)
            {
                ConfigId = settings.ConfigId;

            }
    */
    }
}