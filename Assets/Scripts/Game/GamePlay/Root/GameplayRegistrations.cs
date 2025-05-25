using DI;
using Game.GamePlay.Services;
using Game.State;
using Scripts.Game.GameRoot.Services;

namespace Game.GamePlay.Root
{
    public static class GameplayRegistrations
    {
        /**
         * Регистрируем все сервисы для сцены, вручную
         */
        public static void Register(DIContainer container, GameplayEnterParams gameplayEnterParams)
        {
            container.RegisterFactory(c => new SomeGameplayService(
                c.Resolve<IGameStateProvider>().GameState,
                c.Resolve<SomeCommonService>())
            ).AsSingle();
        }
    }
}