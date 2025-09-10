

using System;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.WaveStates;
using Game.GamePlay.Services;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.Waves
{
    public class GateWaveViewModel
    {

        public ReactiveProperty<Vector2> Position = new();
        public ReactiveProperty<Vector2Int> Direction = new();

        public ReactiveProperty<bool> ShowGateWave;
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
            });
            
            
        }
        
        
    }
}