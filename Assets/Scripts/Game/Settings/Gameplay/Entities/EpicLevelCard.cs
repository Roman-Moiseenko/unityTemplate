using System;
using System.Collections.Generic;
using Game.State.Common;

namespace Game.Settings.Gameplay.Entities.Tower
{
    [Serializable]

    public class EpicLevelCard
    {
        public TypeEpic Level;
        public List<ParameterSettings> UpgradeParameters;
        public List<ParameterSettings> LevelCardParameters;
    }
}