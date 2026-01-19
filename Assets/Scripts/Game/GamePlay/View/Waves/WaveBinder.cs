using R3;
using UnityEngine;

namespace Game.GamePlay.View.Waves
{
    public class WaveBinder : MonoBehaviour
    {
        public WaveViewModel _waveViewModel;
        public void Bind(WaveViewModel waveViewModel)
        {
            _waveViewModel = waveViewModel;
            
        }

    }
}