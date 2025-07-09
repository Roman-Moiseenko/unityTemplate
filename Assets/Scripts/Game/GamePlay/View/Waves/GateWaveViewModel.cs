

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

        public ReactiveProperty<bool> ShowGate;
        public GateWaveViewModel(WaveService waveService)
        {
            ShowGate = new ReactiveProperty<bool>(true);
            
            _waveService = waveService;
            ShowGate = _waveService.ShowGate;
            _waveService.StartForced.Subscribe(newValue =>
            {
                //TODO если true => ворота иначе инфо поле
            });
            //_waveService

        }


        public void ShowGateModel()
        {
            ShowGate.Value = false;
        }

        public void ShowInfoModel()
        {
            ShowGate.Value = true;
        }
    }
}