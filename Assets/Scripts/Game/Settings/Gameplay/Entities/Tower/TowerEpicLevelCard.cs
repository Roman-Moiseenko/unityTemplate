using System;
using System.Collections.Generic;

namespace Game.Settings.Gameplay.Entities.Tower
{
    [Serializable]

    public class TowerEpicLevelCard
    {
        public int Level;
        public List<TowerParameterSettings> UpgradeParameters;
        public List<TowerParameterSettings> LevelCardParameters;
    }
}