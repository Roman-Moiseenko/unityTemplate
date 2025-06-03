using DI;
using Game.GamePlay.Services;
using Game.GamePlay.View.UI;
using Scripts.Game.GameRoot.Services;

namespace Game.GamePlay.Root.View
{
    /**
     * Регистрация view-моделей сцены
     */
    public static class GameplayViewModelsRegistrations
    {
        public static void Register(DIContainer container)
        {
            //Добавить сервис если нужен в UIGameplayRootViewModel и WorldGameplayRootViewModel
            container.RegisterFactory(c => new GameplayUIManager(container)).AsSingle();
            container.RegisterFactory(c => new UIGameplayRootViewModel()).AsSingle();
            container.RegisterFactory(c => new WorldGameplayRootViewModel(
                c.Resolve<BuildingsService>(),
                c.Resolve<GroundsService>(),
                c.Resolve<ResourcesService>(),
                c.Resolve<TowersService>()
                )).AsSingle();
        }
    }
}