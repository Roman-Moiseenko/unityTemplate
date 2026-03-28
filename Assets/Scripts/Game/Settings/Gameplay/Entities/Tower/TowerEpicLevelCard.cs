using System;
using System.Collections.Generic;
using Game.State.Common;
using Game.State.Inventory;

namespace Game.Settings.Gameplay.Entities.Tower
{
    [Serializable]

    public class TowerEpicLevelCard
    {
        public TypeEpic Level;
        public List<TowerParameterSettings> UpgradeParameters;
        public List<TowerParameterSettings> LevelCardParameters;
    }
}