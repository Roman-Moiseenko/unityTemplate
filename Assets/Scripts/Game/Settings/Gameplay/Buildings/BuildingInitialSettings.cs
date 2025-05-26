using System;
using UnityEngine;

namespace Game.Settings.Gameplay.Buildings
{
    [Serializable]
    public class BuildingInitialSettings
    {
        public string TypeId;
        public int Level;
        public Vector3Int Position;
    }
}