using Game.Settings.Gameplay.Entities;

namespace Game.State.Parameters
{

    public class ParameterData
    {
        public ParameterType ParameterType;   // "Damage", "Health", "Distance"
        public float Value;

        public ParameterData() { }

        public ParameterData(ParameterType parameterType, float value)
        {
            ParameterType = parameterType;
            Value = value;
        }

        public ParameterData(ParameterSettings parameterSettings)
        {
            ParameterType = parameterSettings.ParameterType;
            Value = parameterSettings.Value;
        }

        public ParameterData GetCopy()
        {
            return new ParameterData
            {
                Value = this.Value,
                ParameterType = this.ParameterType,
            };
        }
        
    }
}