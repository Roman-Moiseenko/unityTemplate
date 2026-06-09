using System;
using Game.State.Parameters;

namespace Game.Settings.Gameplay.Entities
{
    [Serializable]
    public class LevelCardParameters
    {
        public ParameterType ParameterType;
        public float BaseValue;
        public float PowEpic;
    }
}