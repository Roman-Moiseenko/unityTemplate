using System.Collections.Generic;
using Game.State.Maps.Mobs;
using UnityEngine;

namespace Game.Settings.Gameplay.Enemies
{
    public class MobSettings
    {
        public string ConfigId;
        public string TitleLid;

        public string DescriptionLid;
        //  public MobType Type { get; private set; }

        public string PrefabPath;

        // public float Speed { get; private set; }
        public float SpeedMove;
        public float SpeedAttack;
        public bool IsFly;
        public int AvailableWave;

        public float Health;
        public float Armor;
        public float Attack;
        public int RewardCurrency;

        public MobDefence Defence;

        //Кол-во мобов на 1 единицу
        public int Count = 1;
        //public MobParameters BaseParameters  { get; private set; }
        //   public List<MobParameters> Parameters { get; private set; }
    }
}