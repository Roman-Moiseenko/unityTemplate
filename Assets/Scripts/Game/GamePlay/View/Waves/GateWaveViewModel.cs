

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

        public ReactiveProperty<bool> ShowInfo;
        public GateWaveViewModel(WaveService waveService)
        {
            ShowInfo = new ReactiveProperty<bool>(true);
            
            _waveService = waveService;
            _waveService.StartForced.Subscribe(newValue =>
            {
                //TODO если true => ворота иначе инфо поле
            });

        }
        

        
    }
}