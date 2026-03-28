using System;
using Game.State.Maps.Skills;

namespace Game.Settings.Gameplay.Entities.Skill
{
    [Serializable]
    public class SkillParameterSettings
    {
        public SkillParameterType ParameterType;
        public float Value;
    }
}