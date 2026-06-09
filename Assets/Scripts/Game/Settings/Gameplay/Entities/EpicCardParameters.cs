using System;
using System.Collections.Generic;
using Game.State.Parameters;

namespace Game.Settings.Gameplay.Entities
{
    [Serializable]
    public class EpicCardParameters
    {
        public ParameterType ParameterType;
        public List<EpicParameters> EpicParameters;
    }
}