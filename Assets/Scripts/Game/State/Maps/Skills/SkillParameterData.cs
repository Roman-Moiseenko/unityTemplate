using Game.Settings.Gameplay.Entities.Skill;

namespace Game.State.Maps.Skills
{
    public class SkillParameterData
    {
        public SkillParameterType ParameterType;
        public float Value;

        public SkillParameterData()
        {
            
        }
        
        public SkillParameterData(SkillParameterSettings parameterSetting)
        {
            ParameterType = parameterSetting.ParameterType;
            Value = parameterSetting.Value;
        }
        
        public SkillParameterData GetCopy()
        {
            return new SkillParameterData
            {
                Value = this.Value,
                ParameterType = this.ParameterType
            };
        }
    }
}