using DI;
using Game.GamePlay.View.UI.PanelGateWave.InfoTower;
using Game.GamePlay.View.UI.PanelGateWave.InfoWave;
using MVVM.UI;

namespace Game.GamePlay.View.UI.PanelGateWave
{
    public class PanelGateWaveViewModel : PanelViewModel
    {
        public InfoWaveViewModel InfoWaveViewModel;
        public InfoTowerViewModel InfoTowerViewModel;
        public override string Id => "PanelGateWave";
        public override string Path => "Gameplay/Panels/GateWaveInfo/";
        
        public PanelGateWaveViewModel(
            GameplayUIManager uiManager, 
            DIContainer container
            ) : base(container)
        {

            InfoWaveViewModel = new InfoWaveViewModel(container);
            InfoTowerViewModel = new InfoTowerViewModel(container);
        }

    }
}