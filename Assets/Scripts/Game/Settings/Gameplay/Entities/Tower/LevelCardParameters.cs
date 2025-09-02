using System;
using Game.State.Maps.Towers;

namespace Game.Settings.Gameplay.Entities.Tower
{
    [Serializable]
    public class LevelCardParameters
    {
        public TowerParameterType ParameterType;
        public float BaseValue;
        public float PowEpic;
    }
}