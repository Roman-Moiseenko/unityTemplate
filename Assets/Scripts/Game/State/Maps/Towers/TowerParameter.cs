using System;
using Cysharp.Threading.Tasks;
using R3;

namespace Game.State.Maps.Towers
{
    public class TowerParameter : IDisposable
    {
        public TowerParameterData Origin;
        public TowerParameterType ParameterType;
        public ReactiveProperty<float> Value;
        private DisposableBag _disposables;

        public TowerParameter(TowerParameterData data)
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