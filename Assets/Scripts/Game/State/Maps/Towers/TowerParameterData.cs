using Game.Settings.Gameplay.Entities.Tower;

namespace Game.State.Maps.Towers
{
    public class TowerParameterData
    {
        public TowerParameterType ParameterType;
        public float Value;

        public TowerParameterData() { }
        public TowerParameterData(TowerParameterType parameterType, float value)
        {
            ParameterType = parameterType;
            Value = value;
        }
        public TowerParameterData(TowerParameterSettings parameterSetting)
        {
            ParameterType = parameterSetting.ParameterType;
            Value = parameterSetting.Value;
        }
    }
}