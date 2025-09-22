using DI;
using MVVM.UI;

namespace Game.MainMenu.View.ScreenPlay.PopupProfile
{
    public class PopupProfileViewModel : WindowViewModel
    {
        private readonly DIContainer _container;


        public override string Id => "PopupProfile";
        public override string Path => "MainMenu/ScreenPlay/Popups/";

        public PopupProfileViewModel(DIContainer container)
        {
            _container = container;
            //TODO получаем все настройки и др. для передачи в Binder
            
        }
    }
}