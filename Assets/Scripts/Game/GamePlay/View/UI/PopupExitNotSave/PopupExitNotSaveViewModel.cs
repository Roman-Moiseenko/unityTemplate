using DI;
using Game.GamePlay.Root;
using Game.GamePlay.Services;
using MVVM.UI;
using R3;

namespace Game.GamePlay.View.UI.PopupExitNotSave
{
    public class PopupExitNotSaveViewModel : WindowViewModel
    {
        public override string Id => "PopupExitNotSave";
        public override string Path => "Gameplay/Popups/";

        public PopupExitNotSaveViewModel(
            DIContainer container) : base(container)
        {
            
        }

        public void RequestExit()
        {
            Container.Resolve<GameplayService>().Abort();
        }

        public void RequestContinue()
        {
            RequestClose();
        }
    }
}