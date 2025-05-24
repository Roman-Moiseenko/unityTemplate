using DI;
using Game.MainMenu.Services;

namespace Game.MainMenu.Root.View
{
    public class MainMenuViewModelsRegistrations
    {
        public static void Register(DIContainer container)
        {
            container.RegisterFactory(c => new UIMainMenuRootViewModel(container.Resolve<SomeMainMenuService>())).AsSingle();
        }
    }
}