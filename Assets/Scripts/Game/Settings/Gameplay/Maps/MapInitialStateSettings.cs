using System;
using System.Collections.Generic;
using Game.Settings.Gameplay.Buildings;
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
        //public CastleInitialSettings Castle;
        public List<BuildingInitialSettings> Buildings;
        public List<GroundInitialSettings> Grounds;
        
        public List<TowerInitialSettings> Towers;

        public List<RoadInitialSettings> WayMain;
        public List<RoadInitialSettings> WaySecond;
        public List<RoadInitialSettings> WayDisabled;
        
        public string GroundDefault;
        //Доп.настройки карты
    }
}
