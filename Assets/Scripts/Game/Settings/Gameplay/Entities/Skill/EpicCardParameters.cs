using System;
using System.Collections.Generic;

using Game.State.Maps.Skills;

namespace Game.Settings.Gameplay.Entities.Skill
{
    [Serializable]
    public class EpicCardParameters
    {
        public SkillParameterType ParameterType;
        public List<EpicParameters> EpicParameters;
    }
}