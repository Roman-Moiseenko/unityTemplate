using System;
using System.Collections.Generic;
using Game.Settings.Gameplay.Enemies;
using Game.Settings.Gameplay.Entities.Road;
using Game.Settings.Gameplay.Entities.Tower;
using Game.Settings.Gameplay.Grounds;
using Game.State.Maps.Grounds;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Settings.Gameplay.Maps
{
    [Serializable]
    public class MapInitialStateSettings
    {
     //   public List<BuildingInitialSettings> Buildings;
     //   public List<GroundInitialSettings> Grounds;
        
        public List<TowerInitialSettings> Towers;

   //     public List<RoadInitialSettings> WayMain;
   //     public List<RoadInitialSettings> WaySecond;
     //   public List<RoadInitialSettings> WayDisabled;

        public List<WaveSettings> Waves;
        
        public string groundDefault;
        public bool smallMap = false;
        public bool hasWaySecond = false;
        public bool hasWayDisabled = false;
        public bool obstacle = false; //Препятствия
        public int collapse = 0; //Степень провалов на начальной карте
        public string roadDefault = "Road";
        //Доп.настройки карты
    }
}
