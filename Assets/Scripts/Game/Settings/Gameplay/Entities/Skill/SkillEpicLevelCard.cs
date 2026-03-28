using System;
using System.Collections.Generic;
using Game.Settings.Gameplay.Entities.Tower;
using Game.State.Common;
using Game.State.Inventory;

namespace Game.Settings.Gameplay.Entities.Skill
{
    [Serializable]

    public class SkillEpicLevelCard
    {
        public TypeEpic Level;
        public List<SkillParameterSettings> UpgradeParameters;
        public List<SkillParameterSettings> LevelCardParameters;
    }
}