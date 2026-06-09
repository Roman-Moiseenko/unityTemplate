using System;
using Game.State.Parameters;

namespace Game.Settings.Gameplay.Entities
{

    [Serializable]
    public class ParameterSettings
    {
        public ParameterType ParameterType;   
        public float Value;
    }
}