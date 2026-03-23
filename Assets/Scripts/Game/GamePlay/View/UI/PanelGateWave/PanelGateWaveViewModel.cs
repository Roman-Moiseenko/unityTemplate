using DI;
using Game.GamePlay.View.UI.PanelGateWave.InfoTower;
using Game.GamePlay.View.UI.PanelGateWave.InfoWave;
using Game.State;
using MVVM.UI;

namespace Game.GamePlay.View.UI.PanelGateWave
{
    public class PanelGateWaveViewModel : PanelViewModel
    {
        public InfoWaveViewModel InfoWaveViewModel;
        public InfoWaveViewModel InfoSecondWaveViewModel;
        public InfoTowerViewModel InfoTowerViewModel;
        public bool HasSecondWay;
        public override string Id => "PanelGateWave";
        public override string Path => "Gameplay/Panels/GateWaveInfo/";
        
        public PanelGateWaveViewModel(
            GameplayUIManager uiManager, 
            DIContainer container
            ) : base(container)
        {

            InfoWaveViewModel = new InfoWaveViewModel(container);
            InfoTowerViewModel = new InfoTowerViewModel(container);
            var gameplayState = container.Resolve<IGameStateProvider>().GameplayState;
            HasSecondWay = gameplayState.HasWaySecond.CurrentValue;
            if (HasSecondWay) InfoSecondWaveViewModel = new InfoWaveViewModel(container, false);
        }

    }
}