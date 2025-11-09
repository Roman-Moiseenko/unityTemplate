using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Settings.Gameplay.Maps
{
    public class MapsSettings
    {
        public List<MapSettings> Maps = new();
        public List<string> GroundConfigIds = new();
        public List<string> RoadConfigIds = new();

        public InfinitySetting InfinitySetting = new();
    }
}