

using Game.GamePlay.Services;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Waves
{
    public class GateWaveViewModel
    {
        private readonly WaveService _waveService;

        public ReactiveProperty<Vector2> Position = new();
        public ReactiveProperty<Vector2Int> Direction = new();

        public ReactiveProperty<bool> ShowGateWave;
        public GateWaveViewModel(WaveService waveService)
        {
            ShowGateWave = new ReactiveProperty<bool>(true);
            
            _waveService = waveService;
            ShowGateWave = _waveService.ShowGateWave;
            _waveService.StartForced.Subscribe(newValue =>
            {
                //TODO если true => ворота иначе инфо поле
            });
            //_waveService

        }
    }
}