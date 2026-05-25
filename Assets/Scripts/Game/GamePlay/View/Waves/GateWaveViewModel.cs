

using System;
using Cysharp.Threading.Tasks;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.WaveStates;
using Game.GamePlay.Services;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Waves
{
    public class GateWaveViewModel : IDisposable
    {

        private readonly ReactiveProperty<Vector2> _position = new();
        private readonly ReactiveProperty<Vector2Int> _direction = new();
        public ReadOnlyReactiveProperty<Vector2> Position => _position;
        public ReadOnlyReactiveProperty<Vector2Int> Direction => _direction;

        public readonly ReactiveProperty<bool> ShowGateWave;
        private DisposableBag _disposables = new();

        public GateWaveViewModel(FsmWave fsmWave)
        {
            ShowGateWave = new ReactiveProperty<bool>(false);
            
            fsmWave.Fsm.StateCurrent.Subscribe(state =>
            {
                if (state.GetType() == typeof(FsmStateWaveEnd))
                {
                    ShowGateWave.OnNext(false);
                }

                if (state.GetType() == typeof(FsmStateWaveGo))
                {
                    ShowGateWave.OnNext(true);
                }
            }).AddTo(ref _disposables);
        }

        public void SetPosition(Vector2 position)
        {
            _position.Value = position;
        }

        public void SetDirection(Vector2Int direction)
        {
            _direction.Value = direction;
        }
        public void Dispose()
        {
            Position?.Dispose();
            Direction?.Dispose();
            ShowGateWave?.Dispose();
            _disposables.Dispose();
        }
    }
}