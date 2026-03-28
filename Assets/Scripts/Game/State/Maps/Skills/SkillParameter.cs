using R3;

namespace Game.State.Maps.Skills
{
    public class SkillParameter
    {
        public SkillParameterData Origin;
        public SkillParameterType ParameterType;
        public ReactiveProperty<float> Value;

        public SkillParameter(SkillParameterData data)
        {
            Origin = data;
            ParameterType = data.ParameterType;
            Value = new ReactiveProperty<float>(data.Value);
            Value.Subscribe(newValue => data.Value = newValue);
        }
    }
}