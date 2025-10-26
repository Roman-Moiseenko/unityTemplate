using System.Collections.Generic;
using Game.State.Maps.Mobs;
using UnityEngine;

namespace Game.Settings.Gameplay.Enemies
{
    public class MobSettingsWeb
    {
        public string ConfigId { get; private set; }
        public string TitleLid { get; private set; }
        public string DescriptionLid { get; private set; }
      //  public MobType Type { get; private set; }
        
        public string PrefabPath { get; private set; }

       // public float Speed { get; private set; }
        public float SpeedMove { get; private set; }
        public float SpeedAttack { get; private set; }
        public bool IsFly { get; private set; }
        public int AvailableWave{ get; private set; }

        public float Health { get; private set; }
        public float Armor { get; private set; }
        public float Attack { get; private set; }
        public int RewardCurrency { get; private set; }       
        public MobDefence Defence { get; private set; }
        //Кол-во мобов на 1 единицу
        public int Count { get; private set; } = 1; 
        //public MobParameters BaseParameters  { get; private set; }
     //   public List<MobParameters> Parameters { get; private set; }
        

    }
}