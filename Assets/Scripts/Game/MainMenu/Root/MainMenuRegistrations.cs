using DI;
using Game.MainMenu.Services;
using Scripts.Game.GameRoot.Services;

namespace Game.MainMenu.Root
{
    public static class MainMenuRegistrations
    {
        public static void Register(DIContainer container, MainMenuEnterParams mainMenuEnterParams)
        {
            container.RegisterFactory(c => new SomeMainMenuService(c.Resolve<SomeCommonService>())).AsSingle();
        }
    }
}