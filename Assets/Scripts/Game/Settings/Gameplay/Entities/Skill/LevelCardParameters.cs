using System;
using Game.State.Maps.Skills;

namespace Game.Settings.Gameplay.Entities.Skill
{
    [Serializable]
    public class LevelCardParameters
    {
        public SkillParameterType ParameterType;
        public float BaseValue;
        public float PowEpic;
    }
}