using System;
using System.Collections.Generic;
using Game.State.Maps.Towers;

namespace Game.Settings.Gameplay.Entities.Tower
{
    [Serializable]
    public class EpicCardParameters
    {
        public TowerParameterType ParameterType;
        public List<EpicParameters> EpicParameters;
    }
}