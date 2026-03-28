using System;
using System.Collections.Generic;
using Game.Settings.Gameplay.Entities.Tower;

namespace Game.Settings.Gameplay.Entities.Skill
{
    [Serializable]
    public class SkillLevelSettings
    {
        public int Level;
        public List<SkillParameterSettings> Parameters = new();

    }
}