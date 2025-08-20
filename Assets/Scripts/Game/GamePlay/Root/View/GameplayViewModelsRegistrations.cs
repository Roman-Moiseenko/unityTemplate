using DI;
using Game.Common;
using Game.GamePlay.Classes;
using Game.GamePlay.Fsm;
using Game.GamePlay.Services;
using Game.GamePlay.View.UI;
using Game.GamePlay.View.Waves;
using Game.MainMenu.Services;
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
            //Добавить сервис если нужен в UIGameplayRootViewModel и WorldGameplayRootViewModel
            container.RegisterFactory(c => new UIGameplayRootViewModel(container)).AsSingle();
            //container.RegisterFactory(c => new GateWaveViewModel(c.Resolve<WaveService>())).AsSingle();
            container.RegisterFactory(c => new GameplayUIManager(container)).AsSingle();          
            
            //Всегда последний
            container.RegisterFactory(c => new WorldGameplayRootViewModel(
                //   c.Resolve<BuildingsService>(),
                c.Resolve<GroundsService>(),
                c.Resolve<TowersService>(),
                c.Resolve<CastleService>(),
                c.Resolve<FsmGameplay>(),
                c.Resolve<FrameService>(),
                c.Resolve<PlacementService>(),
                c.Resolve<RoadsService>(),
                c.Resolve<WaveService>(),
                c.Resolve<GameplayCamera>(),
                c.Resolve<DamageService>(),
                c.Resolve<ShotService>(),
                container
            )).AsSingle();
            
        }
    }
}