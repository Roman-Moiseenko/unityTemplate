using DI;
using Game.GamePlay.Commands.TowerCommand;
using Game.GamePlay.Fsm;
using MVVM.CMD;
using MVVM.UI;

namespace Game.GamePlay.View.UI.PopupTowerDelete
{
    public class PopupTowerDeleteViewModel :  WindowViewModel
    {
        private readonly ICommandProcessor _cmd;
        private readonly FsmTower _fsmTower;

        public PopupTowerDeleteViewModel(DIContainer container) : base(container)
        {
            _cmd = container.Resolve<ICommandProcessor>();
            _fsmTower = container.Resolve<FsmTower>();
            
        }

        public override string Id => "PopupTowerDelete";
        
        public override string Path => "Gameplay/Popups/";
        

        public void RequestDelete()
        {
            var towerViewModel = _fsmTower.GetTowerViewModel();
            var command = new CommandDeleteTower(towerViewModel.UniqueId);
            _cmd.Process(command);
            RequestClose();

        }

        public void RequestCancel()
        {
            RequestClose();
        }
        
    }
}