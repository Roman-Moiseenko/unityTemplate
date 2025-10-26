using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Settings.Gameplay.Entities.Tower
{
    [Serializable]
    public class TowerLevelSettings
    {
        public int Level;
        public List<TowerParameterSettings> Parameters = new();

    }
}