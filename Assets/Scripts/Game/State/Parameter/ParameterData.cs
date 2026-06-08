using System.Collections.Generic;
using Game.Settings.Gameplay.Entities;

namespace Game.State.Parameter
{

    public class ParameterData
    {
        public ParameterType TypeId;   // "Damage", "Health", "Distance"
        public float Value;

        public ParameterData() { }

        public ParameterData(ParameterType typeId, float value)
        {
            TypeId = typeId;
            Value = value;
        }

        public ParameterData(ParameterSettings parameterSettings)
        {
            TypeId = parameterSettings.ParameterType;
            Value = parameterSettings.Value;
        }

        public ParameterData GetCopy()
        {
            return new ParameterData
            {
                Value = this.Value,
                TypeId = this.TypeId,
            };
        }
        
    }
}