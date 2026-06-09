using System;
using R3;

namespace Game.State.Parameters
{
    /// <summary>
    /// Реактивная обёртка над ParameterData.
    /// Синхронизирует изменения ReactiveProperty с оригинальными данными (Origin).
    /// </summary>
    public class Parameter : IDisposable
    {
        public ParameterData Origin;
        public ParameterType ParameterType;
        public ReactiveProperty<float> Value;
        private DisposableBag _disposables;

        public Parameter(ParameterData data)
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