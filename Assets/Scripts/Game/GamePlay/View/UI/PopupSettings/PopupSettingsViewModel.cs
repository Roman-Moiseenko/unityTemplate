using DI;
using MVVM.UI;

namespace Game.GamePlay.View.UI.PopupSettings
{
    public class PopupSettingsViewModel : WindowViewModel
    {
        public override string Id => "PopupSettings";
        public override string Path => "Gameplay/Popups/";

        public PopupSettingsViewModel(DIContainer container) : base(container)
        {
            
        }
    }
}