using System;
using System.Collections.Generic;
using Game.Settings.Gameplay.Buildings;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Settings.Gameplay.Maps
{
    [Serializable]
    public class MapInitialStateSettings
    {
        public List<BuildingInitialSettings> Buildings;
        //Доп.настройки карты
    }
}
