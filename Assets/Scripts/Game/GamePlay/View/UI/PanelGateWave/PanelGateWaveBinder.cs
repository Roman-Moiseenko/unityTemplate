using System;
using Game.GamePlay.View.UI.PanelGateWave.InfoTower;
using Game.GamePlay.View.UI.PanelGateWave.InfoWave;
using MVVM.UI;
using R3;
using UnityEngine;

namespace Game.GamePlay.View.UI.PanelGateWave
{
    public class PanelGateWaveBinder : PanelBinder<PanelGateWaveViewModel>
    {
        [SerializeField] private InfoWaveBinder infoWave;
        [SerializeField] private InfoWaveBinder infoWaveSecond;
        [SerializeField] private InfoTowerBinder infoTower;
        
        private IDisposable _disposableImplementation;
        
        protected override void OnBind(PanelGateWaveViewModel viewModel)
        {
            infoWave.Bind(viewModel.InfoWaveViewModel);
            infoTower.Bind(viewModel.InfoTowerViewModel);
            
            var d = Disposable.CreateBuilder();
            
            _disposableImplementation = d.Build();
        }
        
        public override void Show()
        {
        }

        public override void Hide()
        {
        }

        public void OnDestroy()
        {
            _disposableImplementation.Dispose();
        }
    }
}