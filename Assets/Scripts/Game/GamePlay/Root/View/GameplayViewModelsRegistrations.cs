using DI;
using Game.Common;
using Game.GamePlay.Classes;
using Game.GamePlay.Services;
using Game.GamePlay.View.Towers;
using Game.GamePlay.View.UI;
using R3;

namespace Game.GamePlay.Root.View
{
    /**
     * Регистрация view-моделей сцены
     */
    public static class GameplayViewModelsRegistrations
    {
        public static void Register(DIContainer container)
        {
            //Сигналы для вызова UI ()

            //Клик по пустому месту (сброс UI)
            container.RegisterFactory(AppConstants.CLICK_WORLD_ENTITY, _ => new Subject<Unit>()).AsSingle();

            //Клик по башне
            container.RegisterFactory(_ => new Subject<TowerViewModel>()).AsSingle();
            
            //Добавить сервис если нужен в UIGameplayRootViewModel и WorldGameplayRootViewModel
            container.RegisterFactory(c => new UIGameplayRootViewModel(container)).AsSingle();
            //container.RegisterFactory(c => new GateWaveViewModel(c.Resolve<WaveService>())).AsSingle();
            var gameplayUIManager = new GameplayUIManager(container);
            container.RegisterInstance(gameplayUIManager);
            container.RegisterDisposableOnSceneExit(gameplayUIManager);
            //Всегда последний
            var worldRoot =  new WorldGameplayRootViewModel(
                //   c.Resolve<BuildingsService>(),
                container.Resolve<GroundsService>(),
                container.Resolve<TowersService>(),
                container.Resolve<CastleService>(),
                container.Resolve<FrameService>(),
                container.Resolve<FramePlacementService>(),
                container.Resolve<FrameSkillService>(),
                container.Resolve<PlacementService>(),
                container.Resolve<RoadsService>(),
                container.Resolve<WaveService>(),
                container.Resolve<GameplayCamera>(),
                container.Resolve<DamageService>(),
                container.Resolve<SkillsService>(),
                container.Resolve<HeroesService>(),
                //c.Resolve<WarriorService>(),
                //c.Resolve<ShotService>(),
                container
            );
            container.RegisterInstance(worldRoot);
            container.RegisterDisposableOnSceneExit(worldRoot);
        }
    }
}