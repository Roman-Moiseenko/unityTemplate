using Game.GamePlay.Services;
using R3;

namespace Game.GamePlay.View.Waves
{
    public class WaveViewModel
    {
        private readonly WaveService _service;
        //public readonly ReactiveProperty<int> CurrentWave;

        public WaveViewModel(
            WaveService service
            )
        {
           // CurrentWave = service.CurrentNumberWave;
            _service = service;
        }
    }
}