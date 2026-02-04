using DI;
using Game.GamePlay.Commands.TowerCommand;
using Game.GamePlay.Fsm;
using Game.GamePlay.Services;
using MVVM.CMD;
using MVVM.UI;

namespace Game.GamePlay.View.UI.PopupTowerDelete
{
    public class PopupTowerDeleteViewModel :  WindowViewModel
    {
        private readonly FsmTower _fsmTower;
        private readonly TowersService _towerService;

        public PopupTowerDeleteViewModel(DIContainer container) : base(container)
        {
            _fsmTower = container.Resolve<FsmTower>();
            _towerService = container.Resolve<TowersService>();

        }

        public override string Id => "PopupTowerDelete";
        
        public override string Path => "Gameplay/Popups/";
        

        public void RequestDelete()
        {
            var towerViewModel = _fsmTower.GetTowerViewModel();
            _towerService.DeleteTower(towerViewModel.UniqueId);
            
            RequestClose();

        }

        public void RequestCancel()
        {
            RequestClose();
        }
        
    }
}