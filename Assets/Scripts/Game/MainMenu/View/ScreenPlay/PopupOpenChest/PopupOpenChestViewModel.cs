using DI;
using MVVM.UI;

namespace Game.MainMenu.View.ScreenPlay.PopupOpenChest
{
    public class PopupOpenChestViewModel : WindowViewModel
    {
        private readonly DIContainer _container;
        public override string Id => "PopupOpenChest";
        public override string Path => "MainMenu/ScreenPlay/Popups/";

        public PopupOpenChestViewModel(DIContainer container)
        {
            _container = container;
        }
    }
}