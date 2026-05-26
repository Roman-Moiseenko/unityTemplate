using System;
using R3;

namespace Game.State.Maps.Skills
{
    public class SkillParameter : IDisposable
    {
        public SkillParameterData Origin;
        public SkillParameterType ParameterType;
        public ReactiveProperty<float> Value;
        private DisposableBag _disposables;
        public SkillParameter(SkillParameterData data)
        {
            Origin = data;
            ParameterType = data.ParameterType;
            Value = new ReactiveProperty<float>(data.Value);
            Value.Subscribe(newValue => data.Value = newValue).AddTo(ref _disposables);
        }

        public void Dispose()
        {
            Value?.Dispose();
            _disposables.Dispose();
        }
    }
}