using System;
using System.Collections.Generic;
using Game.Settings.Gameplay.Buildings;
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
        public List<BuildingInitialSettings> Buildings;
        public List<GroundInitialSettings> Grounds;
        
        public List<TowerInitialSettings> Towers;

        public string GroundDefault;
        //Доп.настройки карты
    }
}
