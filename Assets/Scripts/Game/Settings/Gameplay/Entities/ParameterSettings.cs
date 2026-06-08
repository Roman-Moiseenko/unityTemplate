using System;
using Game.State.Parameter;

namespace Game.Settings.Gameplay.Entities
{

    [Serializable]
    public class ParameterSettings
    {
        public ParameterType ParameterType;   
        public float Value;
    }
}