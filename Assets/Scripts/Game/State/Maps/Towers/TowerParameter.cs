using R3;

namespace Game.State.Maps.Towers
{
    public class TowerParameter
    {
        public TowerParameterType ParameterType;
        public ReactiveProperty<float> Value;

        public TowerParameter(TowerParameterData data)
        {
            ParameterType = data.ParameterType;
            Value = new ReactiveProperty<float>(data.Value);
            Value.Subscribe(newValue => data.Value = newValue);
        }
    }
}