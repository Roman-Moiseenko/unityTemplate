using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Settings.Gameplay.Buildings
{
    [Serializable]
    public class BuildingInitialSettings
    {
        public string ConfigId;
        public int Level;
        public Vector2Int Position;
    }
}